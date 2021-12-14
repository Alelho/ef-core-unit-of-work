using EFCoreDataAccess.Interfaces;
using EFCoreDataAccess.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;

namespace EFCoreDataAccess.UnitOfWorkImp
{
    public partial class UnitOfWork<T> : IUnitOfWork<T> where T : DbContext
    {
        private IDbContextTransaction _transaction;

        public UnitOfWork(T dbContext)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public DbContext DbContext { get; private set; }

        public TRepository GetRepository<TRepository>() where TRepository : class
        {
            var repositoryType = typeof(TRepository);

            if (repositoryType.IsInterface)
            {
                throw new InvalidOperationException($"The type {nameof(TRepository)} should be a class");
            }

            if (repositoryType.IsAbstract)
            {
                throw new InvalidOperationException($"Could not create an object from an abstract class");
            }

            return (TRepository)Activator.CreateInstance(repositoryType, (DbContext)DbContext);
        }

        public IGenericRepository<TEntity> GetGenericRepository<TEntity>() where TEntity : class
        {
            return new GenericRepository<TEntity>(DbContext);
        }

        #region Dispose Members

        private bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed || !disposing) return;

            DbContext?.Dispose();

            _transaction?.Dispose();

            _disposed = true;
        }

        #endregion
    }
}