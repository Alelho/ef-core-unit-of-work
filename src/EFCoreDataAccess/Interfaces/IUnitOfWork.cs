using Microsoft.EntityFrameworkCore;
using System;

namespace EFCoreDataAccess.Interfaces
{
    public interface IUnitOfWork<T> : ISyncUnitOfWork<T>, IAsyncUnitOfWork<T>, IDisposable where T : DbContext
    {
        IGenericRepository<TEntity> GetGenericRepository<TEntity>() where TEntity : class;
        TRepository GetRepository<TRepository>() where TRepository : class;
    }
}
