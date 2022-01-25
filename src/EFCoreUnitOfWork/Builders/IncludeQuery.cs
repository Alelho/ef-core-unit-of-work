using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EFCoreUnitOfWork.Builders
{
	public class IncludeQuery<TEntity>
	{
		private readonly ICollection<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>> _includeQueries;

		public IncludeQuery()
		{
			_includeQueries = new List<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>>();
		}

		public ICollection<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>> IncludeQueries => _includeQueries;

		public static IncludeQuery<TEntity> Builder()
			=> new();

		public IncludeQuery<TEntity> Include(
			Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeQuery)
		{
			if (includeQuery == null) throw new ArgumentNullException(nameof(includeQuery));

			_includeQueries.Add(includeQuery);

			return this;
		}
	}
}
