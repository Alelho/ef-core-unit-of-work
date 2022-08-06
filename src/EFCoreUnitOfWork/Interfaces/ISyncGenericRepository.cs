using EFCoreUnitOfWork.Builders;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace EFCoreUnitOfWork.Interfaces
{
	public interface ISyncGenericRepository<T> : IGenericRepository where T : class
    {
        T Add(T entity);
        void AddRange(IEnumerable<T> entities);
        void Update(T entity, params Expression<Func<T, object>>[] properties);
        void UpdateRange(IEnumerable<T> entities);
        void RemoveByEntity(T entity);
        void RemoveSingle(Expression<Func<T, bool>> predicate);
        void RemoveRange(IEnumerable<T> entities);

        T SingleOrDefault(Expression<Func<T, bool>> predicate);
        T SingleOrDefault(Expression<Func<T, bool>> predicate, IncludeQuery<T> includeQuery);
        T FirstOrDefault(Expression<Func<T, bool>> predicate);
        T FirstOrDefault(Expression<Func<T, bool>> predicate, IncludeQuery<T> includeQuery);
        T LastOrDefault(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> keySelector);
        T LastOrDefault(
            Expression<Func<T, bool>> predicate,
            Expression<Func<T, object>> keySelector,
            IncludeQuery<T> includeQuery);
        IEnumerable<T> Search(Expression<Func<T, bool>> predicate);
        IEnumerable<T> Search(Expression<Func<T, bool>> predicate, IncludeQuery<T> includeQuery);
        bool Any(Expression<Func<T, bool>> predicate = null);
        long Count(Expression<Func<T, bool>> predicate = null);
        int SaveChanges(bool acceptAllChangesOnSuccess = true);
    }
}
