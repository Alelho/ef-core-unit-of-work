using System;

namespace EFCoreUnitOfWork.Interfaces
{
	public interface IGenericRepository : IDisposable
    { }

    public interface IGenericRepository<T> : ISyncGenericRepository<T>, IAsyncGenericRepository<T> where T : class
    { }
}
