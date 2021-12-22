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

        public virtual bool Any(Expression<Func<T, bool>> predicate = null)
        {
            return predicate == null ?
                _dbSet.Any() :
                _dbSet.Any(predicate);
        }

        public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            return predicate == null ?
                await _dbSet.AnyAsync().ConfigureAwait(continueOnCapturedContext: false) :
                await _dbSet.AnyAsync(predicate).ConfigureAwait(continueOnCapturedContext: false);
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
                await _dbSet.AsNoTracking().LongCountAsync() :
                await _dbSet.AsNoTracking().LongCountAsync(predicate);
        }

        public virtual T FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.FirstOrDefault(predicate);
        }

        public virtual async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);
        }

        public virtual IEnumerable<T> Search(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate)
                 .ToList();
        }

        public async virtual Task<IEnumerable<T>> SearchAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbSet.Where(predicate)
                .ToListAsync(cancellationToken);
        }

        public virtual T SingleOrDefault(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.SingleOrDefault(predicate);
        }

        public virtual async Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbSet.SingleOrDefaultAsync(predicate, cancellationToken);
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

            _disposed = true;
        }

        #endregion
    }
}
