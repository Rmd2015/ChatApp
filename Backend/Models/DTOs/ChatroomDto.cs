using System.ComponentModel.DataAnnotations;

namespace Backend.Models.DTOs
{
    public class ChatroomDto
    {
        public long Id { get; set; }
        public long CreatedByUserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        [Required]
        [StringLength(30)]
        public string ChatRoomLabel { get; set; } = string.Empty;
    }
        // Pour créer un salon
        public class CreateChatroomDto
    {
        [Required]
        public long CreatedByUserId { get; set; }
        [Required]
        [StringLength(30)]
        public string ChatRoomLabel { get; set; } = string.Empty;
    }
}