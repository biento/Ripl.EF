using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ripl.EF.Contracts
{
    public interface ICommandRepository
    {
        TEntity AddEntity<TEntity>(TEntity entity) where TEntity : class;
        TEntity UpdateEntityAsync<TEntity>(TEntity entity) where TEntity : class;
        Task<int> ExecuteCommand(string commandName, params object[] parameters);
        Task<bool> Save(bool dispose = false);
    }
}
