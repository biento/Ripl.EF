using Ripl.EF.Contracts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ripl.EF.Contracts
{
    public abstract class BaseEntity : IEntity
    {
        public bool IsNew => Id.Equals(0);

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
    }
}
