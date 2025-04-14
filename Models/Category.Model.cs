using System.ComponentModel.DataAnnotations;
using Project.Core.Base;

namespace Project.Models
{
    public class Category : BaseEntity<Guid>
    {
        [Required]
        public required string Name { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }
}
