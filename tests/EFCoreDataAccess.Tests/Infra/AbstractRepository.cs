using EFCoreDataAccess.Models;
using EFCoreDataAccess.Repository;
using Microsoft.EntityFrameworkCore;

namespace EFCoreDataAccess.Tests.Infra
{
    public abstract class AbstractRepository : GenericRepository<Employee>
    {
        public AbstractRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
