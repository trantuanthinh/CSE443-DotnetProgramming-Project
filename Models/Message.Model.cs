using Project.Core.Base;
using System.ComponentModel.DataAnnotations;

namespace Project.Models
{
    public class Message : BaseEntity<Guid>
    {
        [Required]
        public required Guid ConversationId { get; set; }

        [Required]
        public required Guid SenderId { get; set; }

        public string? Content { get; set; }
        public DateTime Timestamp { get; set; }
        public bool Is_Read { get; set; }

        public User? Sender { get; set; }
    }
}
