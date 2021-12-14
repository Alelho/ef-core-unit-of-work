using System;
using System.Threading;
using System.Threading.Tasks;

namespace EFCoreDataAccess.UnitOfWorkImp
{
    public partial class UnitOfWork<T>
    {
        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
            {
                throw new InvalidOperationException("There is already an active transaction");
            }

            _transaction = await DbContext.Database.BeginTransactionAsync(cancellationToken)
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                if (_transaction == null)
                {
                    throw new InvalidOperationException("There is already an active transaction");
                }

                await _transaction.CommitAsync(cancellationToken)
                    .ConfigureAwait(continueOnCapturedContext: false);
            }
            catch
            {
                await RollbackAsync(cancellationToken);

                throw;
            }
        }

        public async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                if (_transaction != null)
                {
                    await _transaction.RollbackAsync(cancellationToken)
                        .ConfigureAwait(continueOnCapturedContext: false);
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task<int> SaveChangesAsync(
            bool acceptAllChangesOnSuccess = true, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await DbContext.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken)
                    .ConfigureAwait(continueOnCapturedContext: false);
            }
            catch
            {
                throw;
            }
        }
    }
}
