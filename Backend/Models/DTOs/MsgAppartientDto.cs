namespace Backend.Models.DTOs
{
    public class MsgAppartientDto
    {
        public long MessageId { get; set; }
        public long ChatroomId { get; set; }
        public DateTime? SentAt { get; set; }
    }
}