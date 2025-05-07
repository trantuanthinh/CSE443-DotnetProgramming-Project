using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Core.Base.Enity
{
    public class BaseEntity<IdType>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public required IdType Id { get; set; }
    }
}
