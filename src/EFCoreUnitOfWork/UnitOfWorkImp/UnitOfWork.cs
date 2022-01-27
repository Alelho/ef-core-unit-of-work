using EFCoreUnitOfWork.Interfaces;
using EFCoreUnitOfWork.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;

namespace EFCoreUnitOfWork.UnitOfWorkImp
{
	public partial class UnitOfWork<T> : IUnitOfWork<T> where T : DbContext
    {
        private IDbContextTransaction _transaction;

        public UnitOfWork(T dbContext)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public DbContext DbContext { get; private set; }
        public IDbContextTransaction Transaction => _transaction;

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

            return (TRepository)Activator.CreateInstance(repositoryType, DbContext);
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

            _transaction?.Dispose();
            _transaction = null;

            DbContext?.Dispose();
            DbContext = null;

            _disposed = true;
        }

        #endregion
    }
}