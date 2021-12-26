using Microsoft.EntityFrameworkCore;
using System;
using System.Data;

namespace EFCoreDataAccess.UnitOfWorkImp
{
    public partial class UnitOfWork<T>
    {
        public void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            if (_transaction != null)
            {
                throw new InvalidOperationException("There is already an active transaction");
            }

            _transaction = DbContext.Database.BeginTransaction(isolationLevel);
        }

        public void Commit()
        {
            try
            {
                if (_transaction == null)
                {
                    throw new ArgumentNullException(nameof(Transaction));
                }

                _transaction.Commit();
            }
            catch
            {
                Rollback();

                throw;
            }
        }

        public void Rollback()
        {
            try
            {
                _transaction?.Rollback();
            }
            catch
            {

            }
        }

        public int SaveChanges(bool acceptAllChangesOnSuccess = true)
        {
            try
            {
                return DbContext.SaveChanges(acceptAllChangesOnSuccess);
            }
            catch
            {
                throw;
            }
        }
    }
}
