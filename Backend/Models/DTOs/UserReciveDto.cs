namespace Backend.Models.DTOs
{
    public class UserReciveDto
    {
        public long UserId { get; set; }
        public long MessageId { get; set; }
        public DateTime? ReceivedAt { get; set; }
        public bool? IsViewed { get; set; }
        public DateTime? ViewedAt { get; set; }
    }
}