using System.ComponentModel.DataAnnotations;

namespace Backend.Models.DTOs
{
    // DTO pour afficher un utilisateur (réponse API)
    public class UserDto
    {
        public long Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        public bool IsOnline { get; set; }

        public DateTime? CreatedAt { get; set; }
    }

    // DTO pour la création d'un utilisateur (Register)
    public class CreateUserDto
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;
    }

    // DTO pour la connexion (Login)
    public class LoginDto
    {
        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}