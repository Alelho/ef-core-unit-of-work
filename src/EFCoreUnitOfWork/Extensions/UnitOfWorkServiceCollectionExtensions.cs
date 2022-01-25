using EFCoreUnitOfWork.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using EFCoreUnitOfWork.UnitOfWorkImp;
using Microsoft.EntityFrameworkCore;

namespace EFCoreUnitOfWork.Extensions
{
	public static class UnitOfWorkServiceCollectionExtensions
	{
		public static void AddUnitOfWork<T>(
			this IServiceCollection services,
			ServiceLifetime lifetime = ServiceLifetime.Scoped)
			where T : DbContext
		{
			if (services == null) throw new ArgumentNullException(nameof(services));

			switch (lifetime)
			{
				case ServiceLifetime.Singleton:
					services.AddSingleton<IUnitOfWork<T>, UnitOfWork<T>>();
					break;
				case ServiceLifetime.Scoped:
					services.AddScoped<IUnitOfWork<T>, UnitOfWork<T>>();
					break;
				case ServiceLifetime.Transient:
					services.AddTransient<IUnitOfWork<T>, UnitOfWork<T>>();
					break;
			}
		}
	}
}
