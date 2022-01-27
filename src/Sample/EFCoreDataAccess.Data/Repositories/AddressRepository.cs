using EFCoreDataAccess.Models;
using EFCoreDataAccess.Models.Interface;
using EFCoreUnitOfWork.Repository;
using Microsoft.EntityFrameworkCore;

namespace EFCoreDataAccess.Data.Repositories
{
	public class AddressRepository : GenericRepository<Address>, IAddressRepository
    {
        public AddressRepository(DbContext dbContext)
            : base(dbContext)
        { }
    }
}