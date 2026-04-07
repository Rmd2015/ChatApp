using System.ComponentModel.DataAnnotations;

namespace Backend.Models.DTOs
{
    // ==================== DTO POUR AFFICHAGE ====================
    public class MessageDto
    {
        public long Id { get; set; }                    // idmsg
        public long SenderId { get; set; }              // iduser (expéditeur)
        public long? ChatroomId { get; set; }           // Nullable → peut être null (message direct)
        public string? Content { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    // ==================== DTO POUR ENVOYER UN MESSAGE ====================
    public class CreateMessageDto
    {
        [Required]
        public long SenderId { get; set; }              // Qui envoie le message

        // Soit on envoie dans un Chatroom, soit directement à un utilisateur
        public long? ChatroomId { get; set; }           // Optionnel

        public long? ReceiverId { get; set; }           // Optionnel - pour message direct

        [Required]
        [StringLength(5000, ErrorMessage = "Le message ne peut pas dépasser 5000 caractères")]
        public string Content { get; set; } = string.Empty;
    }
}