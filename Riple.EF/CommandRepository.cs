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

        protected virtual TContext DataContext
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

        /// <summary>
        /// Tracked the added entity. Newly added entities will later be committed to the database on Save.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity">Entity to add.</param>
        /// <returns></returns>
        public virtual TEntity AddEntity<TEntity>(TEntity entity) where TEntity : class
        {
            EntityEntry<TEntity> result;

            result = _dataContext.Add(entity);

            return result?.Entity;
        }
                
        /// <summary>
        /// Track the modified entity. Updates will later be committed to the database on Save.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity">Entity to update.</param>
        /// <returns></returns>
        public virtual TEntity UpdateEntityAsync<TEntity>(TEntity entity) where TEntity : class
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            EntityEntry<TEntity> result;
            result = _dataContext.Update<TEntity>(entity);

            return result?.Entity;

        }

        /// <summary>
        /// Saves changes made on the context to the database.
        /// </summary>
        /// <param name="dispose">Set to true to dispose the context after saving, otherwise set to false. Default is false.</param>
        /// <returns></returns>
        public virtual async Task<bool> Save(bool dispose = false)
        {
            int changes = 0;

            using (IDbContextTransaction _transaction = await _dataContext.Database.BeginTransactionAsync().ConfigureAwait(false))
            {
                try
                {

                    changes = await _dataContext.SaveChangesAsync().ConfigureAwait(false);

                    _transaction.Commit();

                }
                catch
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
