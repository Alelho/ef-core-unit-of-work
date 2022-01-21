using EFCoreDataAccess.Builders;
using EFCoreDataAccess.Extensions;
using EFCoreDataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace EFCoreDataAccess.Repository
{
	public class GenericRepository<T> : IGenericRepository<T> where T : class
	{
		private readonly DbSet<T> _dbSet;

		public GenericRepository(DbContext dbContext)
		{
			DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
			_dbSet = dbContext.Set<T>();
		}

		protected DbContext DbContext { get; private set; }

		#region Writer Members

		public virtual T Add(T entity)
		{
			_dbSet.Add(entity);

			return entity;
		}

		public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
		{
			await _dbSet.AddAsync(entity, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);

			return entity;
		}

		public virtual void AddRange(IEnumerable<T> entities)
		{
			_dbSet.AddRange(entities);
		}

		public virtual async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
		{
			await _dbSet.AddRangeAsync(entities, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}

		public virtual void Update(T entity, params Expression<Func<T, object>>[] properties)
		{
			if (properties.IsNullOrEmpty())
			{
				_dbSet.Update(entity);
				return;
			}

			var originalAutoDetectChangesValue = DbContext.ChangeTracker.AutoDetectChangesEnabled;
			SetAutoDetectChanges(enabled: false);

			DbContext.Entry(entity).State = EntityState.Unchanged;

			var modifiedProperty = properties.Select(o => o.GetPropertyInfo());

			var entityProperties = DbContext.Entry(entity).Metadata.GetProperties();

			foreach (var property in entityProperties)
			{
				if (modifiedProperty.Contains(property.PropertyInfo))
				{
					DbContext.Entry(entity).Property(property.Name).IsModified = true;
				}

				else
				{
					DbContext.Entry(entity).Property(property.Name).IsModified = false;
				}
			}

			SetAutoDetectChanges(originalAutoDetectChangesValue);
		}

		public virtual void UpdateRange(IEnumerable<T> entities)
		{
			if (entities.IsNullOrEmpty()) return;

			_dbSet.UpdateRange(entities);
		}

		public virtual void RemoveByEntity(T entity)
		{
			_dbSet.Remove(entity);
		}

		public virtual void RemoveSingle(Expression<Func<T, bool>> predicate)
		{
			var entity = _dbSet.AsTracking().SingleOrDefault(predicate);

			if (entity == null) throw new InvalidOperationException($"Entity not found!");

			_dbSet.Remove(entity);
		}

		public virtual void RemoveRange(IEnumerable<T> entities)
		{
			if (entities.IsNullOrEmpty()) return;

			_dbSet.RemoveRange(entities);
		}

		#endregion

		#region Reader Members

		public virtual bool Any(Expression<Func<T, bool>> predicate = null)
		{
			return predicate == null ?
				_dbSet.Any() :
				_dbSet.Any(predicate);
		}

		public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default)
		{
			return predicate == null ?
				await _dbSet.AnyAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false) :
				await _dbSet.AnyAsync(predicate, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}

		public virtual long Count(Expression<Func<T, bool>> predicate = null)
		{
			return predicate == null ?
				_dbSet.LongCount() :
				_dbSet.LongCount(predicate);
		}

		public virtual async Task<long> CountAsync(Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default)
		{
			return predicate == null ?
				await _dbSet.LongCountAsync(cancellationToken) :
				await _dbSet.LongCountAsync(predicate, cancellationToken);
		}

		public virtual T FirstOrDefault(Expression<Func<T, bool>> predicate)
		{
			return _dbSet.AsNoTracking()
				.FirstOrDefault(predicate);
		}

		public virtual T FirstOrDefault(Expression<Func<T, bool>> predicate, IncludeQuery<T> includeQuery)
		{
			var query = _dbSet.AsNoTracking().AsQueryable();

			query = AddIncludeQueries(query, includeQuery);

			return query.FirstOrDefault(predicate);
		}

		public virtual async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
		{
			return await _dbSet.AsNoTracking()
				.FirstOrDefaultAsync(predicate, cancellationToken);
		}

		public virtual T LastOrDefault(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> keySelector)
		{
			return _dbSet.AsNoTracking()
				.OrderBy(keySelector)
				.LastOrDefault(predicate);
		}

		public virtual T LastOrDefault(
			Expression<Func<T, bool>> predicate,
			Expression<Func<T, object>> keySelector,
			IncludeQuery<T> includeQuery)
		{
			var query = _dbSet.AsNoTracking().AsQueryable();

			query = AddIncludeQueries(query, includeQuery);

			return query.OrderBy(keySelector)
				.LastOrDefault(predicate);
		}

		public virtual async Task<T> LastOrDefaultAsync(
			Expression<Func<T, bool>> predicate,
			Expression<Func<T, object>> keySelector,
			CancellationToken cancellationToken = default)
		{
			return await _dbSet.AsNoTracking()
				.OrderBy(keySelector)
				.LastOrDefaultAsync(predicate, cancellationToken);
		}

		public virtual IEnumerable<T> Search(Expression<Func<T, bool>> predicate)
		{
			return _dbSet.Where(predicate)
				 .AsNoTracking()
				 .ToList();
		}

		public virtual IEnumerable<T> Search(Expression<Func<T, bool>> predicate, IncludeQuery<T> includeQuery)
		{
			var query = _dbSet.AsNoTracking()
				.AsQueryable();

			query = AddIncludeQueries(query, includeQuery);

			return query.Where(predicate)
				.ToList();
		}

		public async virtual Task<IEnumerable<T>> SearchAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
		{
			return await _dbSet.Where(predicate)
				.AsNoTracking()
				.ToListAsync(cancellationToken);
		}

		public virtual T SingleOrDefault(Expression<Func<T, bool>> predicate)
		{
			return _dbSet.AsNoTracking()
				.SingleOrDefault(predicate);
		}

		public virtual T SingleOrDefault(Expression<Func<T, bool>> predicate, IncludeQuery<T> includeQuery)
		{
			var query = _dbSet.AsNoTracking().AsQueryable();

			query = AddIncludeQueries(query, includeQuery);

			return query.SingleOrDefault(predicate);
		}

		public virtual async Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
		{
			return await _dbSet.AsNoTracking()
				.SingleOrDefaultAsync(predicate, cancellationToken);
		}

		#endregion

		private void SetAutoDetectChanges(bool enabled)
		{
			if (DbContext.ChangeTracker.AutoDetectChangesEnabled == enabled) return;

			DbContext.ChangeTracker.AutoDetectChangesEnabled = enabled;
		}

		private static IQueryable<T> AddIncludeQueries(IQueryable<T> query, IncludeQuery<T> includeQuery)
		{
			foreach (var include in includeQuery.IncludeQueries)
			{
				query = include(query);
			}

			return query;
		}

		#region Disposable Members

		private bool _disposed;

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed || !disposing) return;

			DbContext?.Dispose();
			DbContext = null;

			_disposed = true;
		}

		#endregion
	}
}
