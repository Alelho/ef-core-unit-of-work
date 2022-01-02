using System;

namespace EFCoreDataAccess.Interfaces
{
	public interface IGenericRepository : IDisposable
    { }

    public interface IGenericRepository<T> : IGenericRepository, ISyncGenericRepository<T>, IAsyncGenericRepository<T> where T : class
    { }
}
