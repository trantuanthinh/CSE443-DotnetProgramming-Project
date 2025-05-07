namespace Project.DTO
{
    // DONE
    public class MessageRequest
    {
        public Guid? ConversationId { get; set; }
        public Guid SenderId { get; set; }

        public string? Content { get; set; }
    }
}
