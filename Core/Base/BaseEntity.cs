using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Core.Base
{
    public class BaseEntity<IdType>
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public required IdType Id { get; set; }
    }
}
