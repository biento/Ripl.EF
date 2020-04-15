using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Ripl.EF.Contracts
{
    public interface IQueryRepository
    {
        Task<IQueryable<TEntity>> GetAllAsync<TEntity>(Expression<Func<TEntity, bool>> predicate = null, bool trackChanges =  false) 
            where TEntity : class;
        Task<TEntity> GetEntityAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) 
            where TEntity : class;
        Task<bool> FindAnyEntityAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : class;
        IQueryable<TEntity> FilterBy<TEntity>(IQueryable<TEntity> queryableCollection, string parameterName, object value)
         where TEntity : class;
    }
}
