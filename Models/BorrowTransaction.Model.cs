using System.ComponentModel.DataAnnotations;
using Project.Core.Base.Enity;
using Project.Utils;

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
        public Guid? ManagerId { get; set; }

        [Required]
        public required int Quantity { get; set; }
        public DateTime RequestDate { get; set; } // ngày mà lecturer muốn mượn
        public DateTime? ReturnDate { get; set; } // ngày mà lecturer trả
        public DateTime? DueDate { get; set; } // ngày hạn cuối mà lecturer phải trả (default: 7 ngày)
        public string? Note { get; set; }
        public ItemStatus Status { get; set; }

        public Item? Item { get; set; }
        public User? Borrower { get; set; }
        public User? Manager { get; set; }
    }
}
