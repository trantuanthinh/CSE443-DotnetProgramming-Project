using System.ComponentModel.DataAnnotations;
using Project.Core.Base;
using Project.Utils;

namespace Project.Models
{
    public class User : BaseEntity<Guid>
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public int? PhoneNumber { get; set; }

        [Required]
        public required LoginType LoginType { get; set; }

        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public UserType Role { get; set; }
    }
}
