using EFCoreUnitOfWork.Builders;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace EFCoreUnitOfWork.Interfaces
{
	public interface IAsyncGenericRepository<T> : IGenericRepository where T : class
	{
		Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
		Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
		Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
		Task<T> SingleOrDefaultAsync(
			Expression<Func<T, bool>> predicate,
			IncludeQuery<T> includeQuery,
			CancellationToken cancellationToken = default);
		Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
		Task<T> FirstOrDefaultAsync(
			Expression<Func<T, bool>> predicate,
			IncludeQuery<T> includeQuery,
			CancellationToken cancellationToken = default);
		Task<T> LastOrDefaultAsync(Expression<Func<T, bool>> predicate,
			Expression<Func<T, object>> keySelector,
			CancellationToken cancellationToken = default);
		Task<T> LastOrDefaultAsync(Expression<Func<T, bool>> predicate,
			Expression<Func<T, object>> keySelector,
			IncludeQuery<T> includeQuery,
			CancellationToken cancellationToken = default);
		Task<IEnumerable<T>> SearchAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
		Task<IEnumerable<T>> SearchAsync(
			Expression<Func<T, bool>> predicate,
			IncludeQuery<T> includeQuery,
			CancellationToken cancellationToken = default);
		Task<bool> AnyAsync(Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default);
		Task<long> CountAsync(Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default);

	}
}
