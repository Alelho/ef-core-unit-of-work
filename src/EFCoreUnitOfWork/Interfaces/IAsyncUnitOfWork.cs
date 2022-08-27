using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace EFCoreUnitOfWork.Interfaces
{
    public interface IAsyncUnitOfWork<T> where T : DbContext
    {
        Task BeginTransactionAsync(
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, 
            CancellationToken cancellationToken = default);
        Task RollbackAsync(CancellationToken cancellationToken = default);
        Task CommitAsync(CancellationToken cancellationToken = default);
    }
}
