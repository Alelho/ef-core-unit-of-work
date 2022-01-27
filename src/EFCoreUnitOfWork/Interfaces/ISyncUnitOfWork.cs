using Microsoft.EntityFrameworkCore;
using System.Data;

namespace EFCoreUnitOfWork.Interfaces
{
    public interface ISyncUnitOfWork<T> where T : DbContext
    {
        void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
        void Rollback();
        void Commit();
        int SaveChanges(bool acceptAllChangesOnSuccess = true);
    }
}
