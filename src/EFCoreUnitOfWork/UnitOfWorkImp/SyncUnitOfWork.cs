using Microsoft.EntityFrameworkCore;
using System;
using System.Data;

namespace EFCoreUnitOfWork.UnitOfWorkImp
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
                    throw new ArgumentNullException("Transaction");
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
    }
}
