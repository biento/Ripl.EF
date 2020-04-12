using Microsoft.EntityFrameworkCore;
using Ripl.EF.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Ripl.EF
{
    public abstract class QueryRepository<TContext> : IQueryRepository
        where TContext : DbContext
    {
        private bool _disposed = false;
        private readonly TContext _dataContext;

        /// <summary>
        /// Retrieves records that with applied predicate filter.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="predicate">Condition used to filter records. Default is null, in which case it will return all records.</param>
        /// <param name="trackChanges">If true then returned results are tracked on the datacontext. Default is false.</param>
        /// <returns></returns>
        public async Task<IQueryable<TEntity>> GetAllAsync<TEntity>(Expression<Func<TEntity, bool>> predicate = null, bool trackChanges = false) where TEntity : class
        {
            var dbSet = _dataContext.Set<TEntity>();
            if (dbSet == null)
            {
                return null;
            }

            if (predicate == null)
            {
                return trackChanges ? dbSet.AsTracking() : dbSet.AsNoTracking();
            }

            var result = trackChanges ? dbSet.AsTracking().Where<TEntity>(predicate) : dbSet.AsNoTracking().Where<TEntity>(predicate);
            return result;
        }

        public async Task<TEntity> GetEntityAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {

            var dbSet = _dataContext.Set<TEntity>();

            if (dbSet == null) return null;

            var result = await dbSet.AsNoTracking().FirstOrDefaultAsync<TEntity>(predicate);
            return result;
        }

        public async Task<bool> FindAnyEntityAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            var dbSet = _dataContext.Set<TEntity>();

            if (dbSet == null) return false;

            var result = await dbSet.AsNoTracking().AnyAsync(predicate);

            return result;
        }

        public IEnumerable<TEntity> SearchBy<TEntity>(Func<TEntity, bool> predicate) where TEntity : class
        {
            var dbSet = _dataContext.Set<TEntity>();

            if (dbSet == null)
                return null;

            var result = dbSet.AsNoTracking().Where(predicate);

            return result;
        }

        public IEnumerable<TEntity> ExecuteQuery<TEntity>
            (
                string command, 
                bool trackChanges = false, 
                params object[] parameters
            ) 
            where TEntity : class
        {
            var dbSet = _dataContext.Set<TEntity>();

            if (dbSet == null)
                return null;

            var result = trackChanges ? dbSet.AsTracking().FromSql<TEntity>(command, parameters)
                : dbSet.AsNoTracking().FromSql<TEntity>(command, parameters);

            return result;
        }

        /// <summary>
        /// Defered filtering on a queryable collection based on a property
        /// of one of it's items. Currently works only for value types.
        /// </summary>
        /// <typeparam name="TEntity">Type of the individual items in the colleciton.</typeparam>
        /// <typeparam name="TProperty">Propety to be used for filtering.</typeparam>
        /// <param name="queryableCollection">The collection to be filtered.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="value">Value to be searched in the collection.</param>
        /// <returns>IQueryable<typeparamref name="TEntity"/></returns>
        public IQueryable<TEntity> FilterBy<TEntity>(IQueryable<TEntity> queryableCollection, string parameterName, object value)
         where TEntity : class
        {
            // get the type of entity            
            var entityType = typeof(TEntity);

            // get the type of the value object
            var entityProperty = entityType.GetProperty(parameterName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            var propertyType = entityProperty.PropertyType;
            var isNullableParameter = propertyType.IsGenericType && propertyType.GetGenericTypeDefinition().Equals(typeof(Nullable<>));

            if (isNullableParameter)
            {
                var nullableConverter = new NullableConverter(propertyType);
                propertyType = nullableConverter.UnderlyingType;
            }

            // initialize the paramter p =>
            var parameter = Expression.Parameter(entityType, "p");

            // if value is string box to type of the property
            object convertedValue = value;
            if (value is string && value.GetType() != propertyType)
            {
                convertedValue = Convert.ChangeType(value, propertyType);
            }

            // case-insensitive matching when value is string
            if (propertyType == typeof(string))
            {
                var toLower = typeof(String).GetMethod("ToLower", Type.EmptyTypes);

                var dynamicExpression = Expression.Call(
                    Expression.Convert(Expression.Property(parameter, entityProperty), propertyType), toLower);

                var constant = Expression.Constant(convertedValue.ToString().ToLower());
                var expression = Expression.Equal(dynamicExpression, constant);
                Expression<Func<TEntity, bool>> caseInsensitiveLambda = Expression.Lambda<Func<TEntity, bool>>(expression, parameter);

                return queryableCollection.Where(caseInsensitiveLambda);
            }

            var propertyExpression = isNullableParameter
                ? Expression.Property(Expression.Property(parameter, entityProperty), "Value")
                : Expression.Property(parameter, entityProperty);

            var equalityExpression = Expression.Equal(
                    propertyExpression,
                    Expression.Convert(Expression.Constant(convertedValue), propertyType));

            BinaryExpression expressionBody;
            if (isNullableParameter)
            {
                var hasValueExpression = Expression.Property(Expression.Property(parameter, entityProperty), "HasValue");
                expressionBody = Expression.AndAlso(hasValueExpression, equalityExpression);
            }
            else
            {
                expressionBody = equalityExpression;
            }

            Expression<Func<TEntity, bool>> nullableHandlerLambda = Expression.Lambda<Func<TEntity, bool>>(expressionBody, parameter);

            return queryableCollection.Where(nullableHandlerLambda);
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
