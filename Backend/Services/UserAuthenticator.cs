using Backend.Data;
using Backend.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace Backend.Services
{
    public class UserAuthenticator
    {
        private readonly ChatDbContext _context;
        private readonly IConfiguration _configuration;

        public UserAuthenticator(ChatDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // 1. Authentification et génération du token
        public async Task<object?> LoginAsync(string username, string password, string ipAddress, string userAgent)
        {
            var user = await _context.Chatuser.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null || user.Password != password) return null;

            user.Isconnect = true;

            var tokenString = GenerateJwtToken(user);
            var tokenEntry = new Tokens
            {
                Token = tokenString,
                Iduser = user.Iduser,
                Createdat = DateTime.UtcNow,
                Expiresat = DateTime.UtcNow.AddHours(24),
                Isvalid = true,
                Ipaddress = IPAddress.Parse(ipAddress),
                Useragent = userAgent,
                Lastusedat = DateTime.UtcNow
            };

            await _context.Tokens.AddAsync(tokenEntry);
            await _context.SaveChangesAsync();

            return new { token = tokenString };
        }

        // 2. Validation du token (avec vérification IP)
        public async Task<bool> IsTokenValidAsync(string tokenValue, string currentIpAddress)
        {
            var tokenRecord = await _context.Tokens
                .FirstOrDefaultAsync(t => t.Token == tokenValue);

            if (tokenRecord == null || tokenRecord.Isvalid == false)
                return false;

            // Vérification de l'IP
            if (tokenRecord.Ipaddress?.ToString() != currentIpAddress)
                return false;

            // Vérification expiration
            if (tokenRecord.Expiresat < DateTime.UtcNow)
            {
                tokenRecord.Isvalid = false;
                await _context.SaveChangesAsync();
                return false;
            }

            return true;
        }

        // 3. Invalidation (Logout)
        public async Task<bool> InvalidateTokenAsync(string tokenValue)
        {
            var tokenRecord = await _context.Tokens.FirstOrDefaultAsync(t => t.Token == tokenValue);
            if (tokenRecord == null) return false;

            tokenRecord.Isvalid = false;
            tokenRecord.Expiresat = DateTime.UtcNow;

            var user = await _context.Chatuser.FindAsync(tokenRecord.Iduser);
            if (user != null) user.Isconnect = false;

            await _context.SaveChangesAsync();
            return true;
        }

        // 4. Logique interne de génération JWT
        public string GenerateJwtToken(Chatuser user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Iduser.ToString()),
                new Claim(ClaimTypes.Name, user.Username ?? string.Empty),
                new Claim("IsOnline", user.Isconnect?.ToString() ?? "False")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? ""));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}