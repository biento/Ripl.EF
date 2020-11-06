using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Ripl.EF.Contracts
{
    public interface IQuerySpecification<T> where T : IEntity
    {
        bool IsSatisfiedBy(T entity);
        Expression<Func<T, bool>> ToExpression();
    }
}
