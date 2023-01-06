using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Ripl.EF.Contracts
{
    public abstract class SimpleEntity : IEntity<ushort>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public ushort Id { get; set; }
        /// <summary>
        /// 0: Pending Approval
        /// 1: Active
        /// 2: Inactive
        /// 3: For Deletion
        /// </summary>
        public ushort Status { get; set; }
    }
}
