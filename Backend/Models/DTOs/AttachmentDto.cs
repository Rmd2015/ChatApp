using System.ComponentModel.DataAnnotations;

namespace Backend.Models.DTOs
{
    public class AttachmentDto
    {
        public long Id { get; set; }

        [StringLength(30)]
        public string? Size { get; set; }

        [StringLength(150)]
        public string? Path { get; set; }
    }
}