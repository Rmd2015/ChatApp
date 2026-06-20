using Backend.Data;
using Backend.Mappings;
using Backend.Models.DTOs;
using Backend.Models.Entities;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserAuthenticator _auth;
    private readonly ChatDbContext _context;

    public UserController(UserAuthenticator auth, ChatDbContext context)
    {
        _auth = auth;
        _context = context;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<object>> Login([FromBody] LoginDto dto)
    {
        // Récupération des informations de contexte
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
        var userAgent = Request.Headers["User-Agent"].ToString();

        // Appel du service pour authentifier et générer/sauvegarder le token
        var result = await _auth.LoginAsync(dto.Username, dto.Password, ip, userAgent);

        if (result == null)
            return Unauthorized(new { message = "Identifiants incorrects" });

        // 'result' est l'objet anonyme retourné par LoginAsync dans UserAuthenticator
        return Ok(result);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var authHeader = Request.Headers["Authorization"].ToString();
        var token = authHeader.Replace("Bearer ", "").Trim();

        var success = await _auth.InvalidateTokenAsync(token);

        if (!success) return BadRequest(new { message = "Token invalide ou déjà expiré" });

        return Ok(new { success = true, message = "Déconnexion réussie" });
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
    {

        var users = await _context.Chatuser.AsNoTracking().ToListAsync();
        return Ok(users.ToDtoList());
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<UserDto>> GetUserById(long id)
    {
        var user = await _context.Chatuser.AsNoTracking().FirstOrDefaultAsync(u => u.Iduser == id);
        return user != null ? Ok(user.ToDto()) : NotFound("Utilisateur non trouvé");
    }

    [HttpPut("{id}/online")]
    [Authorize]
    public async Task<IActionResult> UpdateOnlineStatus(long id, [FromBody] bool isOnline)
    {
        var user = await _context.Chatuser.FindAsync(id);
        if (user == null) return NotFound();

        user.Isconnect = isOnline;
        await _context.SaveChangesAsync();
        return Ok(new { message = "Statut mis à jour" });
    }
}