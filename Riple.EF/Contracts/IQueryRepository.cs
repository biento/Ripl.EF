using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Ripl.EF.Contracts
{
    public interface IQueryRepository
    {
        Task<IQueryable<TEntity>> GetAllAsync<TEntity>(Expression<Func<TEntity, bool>> predicate = null, bool trackChanges =  false) where TEntity : class;
    }
}
