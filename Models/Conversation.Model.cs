using Project.Core.Base;
using System.ComponentModel.DataAnnotations;

namespace Project.Models
{
    public class Conversation : BaseEntity<Guid>
    {
        [Required]
        public required Guid FirstUserId { get; set; }

        [Required]
        public required Guid SecondUserId { get; set; }

        public User? FirstUser { get; set; }
        public User? SecondUser { get; set; }
        public ICollection<Message>? Messages { get; set; }
    }
}
