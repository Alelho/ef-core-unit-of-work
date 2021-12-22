using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace EFCoreDataAccess.Interfaces
{
    public interface ISyncGenericRepository<T> : IGenericRepository where T : class
    {
        T SingleOrDefault(Expression<Func<T, bool>> predicate);
        T FirstOrDefault(Expression<Func<T, bool>> predicate);
        IEnumerable<T> Search(Expression<Func<T, bool>> predicate);
        bool Any(Expression<Func<T, bool>> predicate = null);
        long Count(Expression<Func<T, bool>> predicate = null);

        T Add(T entity);
        void AddRange(IEnumerable<T> entities);
    }
}
