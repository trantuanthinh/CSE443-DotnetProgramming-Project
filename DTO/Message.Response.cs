namespace Project.DTO
{
    // DONE
    public class MessageResponse
    {
        public Guid ConversationId { get; set; }
        public Guid SenderId { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public bool Is_Read { get; set; }
    }
}
