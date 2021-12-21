using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;

namespace EFCoreDataAccess.Interfaces
{
    public interface IUnitOfWork<T> : ISyncUnitOfWork<T>, IAsyncUnitOfWork<T>, IDisposable where T : DbContext
    {
        DbContext DbContext { get; }
        IDbContextTransaction Transaction { get; }

        IGenericRepository<TEntity> GetGenericRepository<TEntity>() where TEntity : class;
        TRepository GetRepository<TRepository>() where TRepository : class;
    }
}
