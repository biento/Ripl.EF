using Ripl.EF.Contracts;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Ripl.EF.Specifications
{
    public abstract class QuerySpecification<TEntity> : IQuerySpecification<TEntity> where TEntity : IEntity
    {
        public bool IsSatisfiedBy(TEntity entity)
        {
            Func<TEntity, bool> predicate = ToExpression().Compile();
            return predicate(entity);
        }

        public abstract Expression<Func<TEntity, bool>> ToExpression();
    }
}
