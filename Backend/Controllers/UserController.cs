using Backend.Data;
using Backend.Mappings;
using Backend.Models.DTOs;
using Backend.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ChatDbContext _context;
        private readonly IConfiguration _configuration;

        public UserController(ChatDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // ====================== PUBLIC ROUTES ======================

        // POST: api/user/register
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<UserDto>> Register([FromBody] CreateUserDto dto)
        {
            // 1. Validation du modèle
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // 2. Vérification de l'existence (Ligne qui causait l'erreur)
                // Note: Assurez-vous que dans votre DbContext, l'entité User est mappée sur "users" ou "user" (minuscules)
                bool userExists = await _context.Chatuser
                    .AnyAsync(u => u.Username.ToLower() == dto.Username.ToLower());

                if (userExists)
                    return BadRequest(new { message = "Ce nom d'utilisateur est déjà utilisé" });

                // 3. Création de l'entité
                var newUser = new Chatuser
                {
                    Username = dto.Username,
                    // RAPPEL: Hashage obligatoire en production (ex: BCrypt.Net.BCrypt.HashPassword(dto.Password))
                    Password = dto.Password,
                    Isconnect = false,
                    Createdat = DateTime.UtcNow
                };

                // 4. Sauvegarde
                _context.Chatuser.Add(newUser);
                await _context.SaveChangesAsync();

                // 5. Retourner le résultat
                var userDto = newUser.ToDto();

                // Utilisation de Iduser (votre clé primaire dans l'entité)
                return CreatedAtAction(nameof(GetUserById), new { id = newUser.Iduser }, userDto);
            }
            catch (Npgsql.PostgresException ex) when (ex.SqlState == "42P01")
            {
                // Capture spécifique de l'erreur "Table inexistante" pour un debug clair
                return StatusCode(500, new { message = "Erreur de base de données : La table 'users' n'existe pas. Vérifiez vos migrations." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Une erreur interne est survenue", details = ex.Message });
            }
        }
        // POST: api/user/login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> Login([FromBody] LoginDto dto)
        {
            var user = await _context.Chatuser
                .FirstOrDefaultAsync(u => u.Username == dto.Username);

            if (user == null || user.Password != dto.Password)   // À remplacer par BCrypt plus tard
                return Unauthorized(new { message = "Nom d'utilisateur ou mot de passe incorrect" });

            // Générer le JWT Token

            Tokens token = new Tokens
            {
                Token = GenerateJwtToken(user),
                Createdat = DateTime.UtcNow,
                Expiresat = DateTime.UtcNow.AddHours(24),
                Isvalid =  true,
                Iduser  = user.Iduser,
            };
            await _context.Tokens.AddAsync(token);
            user.Isconnect = true;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                token = token.ToDto(),
                user = user.ToDto()
            });
        }
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            // Récupération sécurisée du header Authorization
            if (!Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                return BadRequest(new { message = "Header Authorization manquant" });
            }

            var headerValue = authHeader.ToString().Trim();

            // Vérification du format Bearer
            if (string.IsNullOrWhiteSpace(headerValue) || !headerValue.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new { message = "Le token doit commencer par 'Bearer '" });
            }

            // Extraire le token
            var token = headerValue.Substring(7).Trim();

            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new { message = "Token vide ou invalide" });
            }

            try
            {
                var blackToken = await _context.Tokens
                .FirstOrDefaultAsync(t => t.Token == token && t.Expiresat > DateTime.UtcNow
                           && t.Isvalid == true);
                if (null == blackToken)
                {
                    return BadRequest(new { message = "Token  invalide" });
                }

                blackToken.Isvalid = false;
                blackToken.Expiresat = DateTime.UtcNow;
                // Version corrigée
                var user = await _context.Chatuser
                    .FirstOrDefaultAsync(u => u.Iduser == blackToken.Iduser && u.Isconnect == true);// je fait ca car Isconnect is nullabele value bool? 
                if (user == null) {
                    return BadRequest(new { message = "User invalide" });

                }
                user.Isconnect = false;
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Déconnexion réussie avec succès."
                });
            }
            catch (Exception ex)
            {
                // Log l'erreur si tu as un logger, mais on retourne quand même un succès côté client
                return Ok(new
                {
                    details = ex.Message,
                    success = true,
                    message = "Déconnexion réussie (token invalidé côté client)."
                });
            }
        }
        // ====================== PROTECTED ROUTES ======================

        // GET: api/user (protégé)
        [HttpGet]
        [Authorize]                         // ← Nécessite un token JWT
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
        {
            //  (on a besoin de tracking pour modifier l'entité).----> AsNoTracking usitilser seulement dans le select dans l insert/update no
            var users = await _context.Chatuser.AsNoTracking()// cette ligne est tres important car je fait le select seulement pas update ou insert ---> donc pas besoin de tracking la rqt!!
                .ToListAsync();
            return Ok(users.ToDtoList());
        }

        // GET: api/user/{id} (protégé)
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetUserById(long id)
        {
            var user = await _context.Chatuser.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Iduser == id);

            if (user == null)
                return NotFound(new { message = "Utilisateur non trouvé" });

            return Ok(user.ToDto());
        }

        // PUT: api/user/{id}/online (protégé)
        [HttpPut("{id}/online")]
        [Authorize]
        public async Task<IActionResult> UpdateOnlineStatus(long id, [FromBody] bool isOnline)
        {
            var user = await _context.Chatuser.FindAsync(id);
            if (user == null)
                return NotFound();

            user.Isconnect = isOnline;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Statut mis à jour" });
        }

        // ====================== JWT TOKEN GENERATOR ======================
        private string GenerateJwtToken(Chatuser user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Iduser.ToString()),
                new Claim(ClaimTypes.Name, user.Username ?? string.Empty),
                new Claim("IsOnline", user.Isconnect.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(24),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}