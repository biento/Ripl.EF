using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Ripl.EF.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Ripl.EF
{
    public abstract class CommandRepository<TContext> : ICommandRepository, IDisposable
        where TContext : DbContext
    {
        private bool _disposed = false;
        private readonly TContext _dataContext;

        protected TContext DataContext
        {
            get
            {
                return _dataContext;
            }
        }

        public CommandRepository(TContext dataContext)
        {
            _dataContext = dataContext;
        }

        public TEntity AddEntity<TEntity>(TEntity entity) where TEntity : class
        {
            EntityEntry<TEntity> result;

            result = _dataContext.Add(entity);

            return result?.Entity;
        }

        

        public TEntity UpdateEntityAsync<TEntity>(TEntity entity) where TEntity : class
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            EntityEntry<TEntity> result;
            result = _dataContext.Update<TEntity>(entity);

            return result?.Entity;

        }

        public async Task<bool> Save(bool dispose = false)
        {
            int changes = 0;

            using (IDbContextTransaction _transaction = await _dataContext.Database.BeginTransactionAsync().ConfigureAwait(false))
            {
                try
                {

                    changes = await _dataContext.SaveChangesAsync().ConfigureAwait(false);

                    _transaction.Commit();

                }
                catch (Exception e)
                {

                    _transaction.Rollback();

                }
                finally
                {
                    if (dispose)
                    {
                        Dispose();
                    }   
                }
            }
            return (changes > 0 ? true : false);
        }

        public async Task<int> ExecuteCommand(string commandName, params object[] parameters)
        {
            int changes = 0;

            changes = await _dataContext.Database.ExecuteSqlCommandAsync(commandName, parameters).ConfigureAwait(false);
            return changes;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed && disposing)
            {
                _dataContext.Dispose();
            }

            this._disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
