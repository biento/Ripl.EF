using System;
using System.Collections.Generic;
using System.Text;

namespace Ripl.EF.Contracts
{
    public interface IEntity
    {
        long Id { get; set; }
    }

    public interface IEntity<TId>
    {
        TId Id { get; set; }
    }
}
