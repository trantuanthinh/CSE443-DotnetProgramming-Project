using Project.Core.Base.Enity;
using System.ComponentModel.DataAnnotations;

namespace Project.Models
{
    public class Item : BaseEntity<Guid>
    {
        public Guid CategoryId { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

        [Required]
        public required string Name { get; set; }

        [Required]
        public required int Quantity { get; set; }

        public string Image { get; set; }

        public Category? Category { get; set; }
    }
}
