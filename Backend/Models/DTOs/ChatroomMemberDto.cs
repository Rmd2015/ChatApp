namespace Backend.Models.DTOs
{
    public class ChatroomMemberDto
    {
        public long ChatroomId { get; set; }
        public long UserId { get; set; }
        public DateTime? JoinedAt { get; set; }
    }
}