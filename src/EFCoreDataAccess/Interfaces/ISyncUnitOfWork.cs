using Microsoft.EntityFrameworkCore;

namespace EFCoreDataAccess.Interfaces
{
    public interface ISyncUnitOfWork<T> where T : DbContext
    {
        void BeginTransaction();
        void Rollback();
        void Commit();
        int SaveChanges(bool acceptAllChangesOnSuccess = true);
    }
}
