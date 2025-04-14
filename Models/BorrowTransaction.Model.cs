using Project.Core.Base;
using Project.Utils;
using System.ComponentModel.DataAnnotations;

namespace Project.Models
{
    public class BorrowTransaction : BaseEntity<Guid>
    {
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        [Required]
        public required Guid ItemId { get; set; }

        [Required]
        public required Guid BorrowerId { get; set; }

        [Required]
        public required int Quantity { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public DateTime DueDate { get; set; }
        public string? Note { get; set; }
        public ItemStatus Status { get; set; }

        public Item? Item { get; set; }
        public User? Borrower { get; set; }
    }
}
