using System;

namespace EFCoreDataAccess.UnitOfWorkImp
{
    public partial class UnitOfWork<T>
    {
        public void BeginTransaction()
        {
            if (_transaction != null)
            {
                throw new InvalidOperationException("There is already an active transaction");
            }

            _transaction = DbContext.Database.BeginTransaction();
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
