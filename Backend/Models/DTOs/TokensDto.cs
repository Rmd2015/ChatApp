using System.ComponentModel.DataAnnotations;

namespace Backend.Models.DTOs
{
    public class TokensDto
    {
        public long Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public bool IsValid { get; set; }
    }


}
