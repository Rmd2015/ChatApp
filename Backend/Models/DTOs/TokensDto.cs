using System.ComponentModel.DataAnnotations;

namespace Backend.Models.DTOs
{
    // DTO principal pour afficher un token blacklisté
    public class TokensDto
    {
        public long Id { get; set; }               // idtoken

        public long UserId { get; set; }           // iduser (important !)

        [StringLength(1000)]
        public string Token { get; set; } = string.Empty;

        public DateTime ExpiresAt { get; set; }

        public DateTime? CreatedAt { get; set; }

        public bool IsValid { get; set; }
    }

    public class TokensDtoForSelect
    {
        [StringLength(1000)]
        public string Token { get; set; } = string.Empty;
    }

}

