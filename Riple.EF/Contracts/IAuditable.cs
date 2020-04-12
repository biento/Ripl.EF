using System;
using System.Collections.Generic;
using System.Text;

namespace Ripl.EF.Contracts
{
    public interface IAuditable
    {
        DateTime? CreatedDate { get; set; }
        DateTime? ModifiedDate { get; set; }
        string CreatedBy { get; set; }
        string ModifiedBy { get; set; }
    }
}
