using EFCoreDataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using EFCoreUnitOfWork.Extensions;
using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using System;

namespace EFCoreDataAccess.API.Configurations
{
	public static class Database
	{
		public static void AddDatabase(this IServiceCollection services)
		{
			var connectionString = @"Server=localhost;Database=EFCoreUnitOfWork;Uid=root;Pwd=123456;";

			// Register DbContext into DI container
			// It can use SQL Server, PostgreSQL instead of MySQL
			services.AddDbContext<EmployeeDbContext>(options =>
				options.UseMySql(connectionString, serverVersion: ServerVersion.AutoDetect(connectionString))
				.LogTo(msg => Debug.WriteLine(msg), LogLevel.Error));

			// Add unit of work into DI container.
			// This is an exetensions from the EfCoreUnitOfWork package
			services.AddUnitOfWork<EmployeeDbContext>();
		}

		public static void ConfigureDatabase(IApplicationBuilder app)
		{
			RunMigrate(app.ApplicationServices);
		}

		private static void RunMigrate(IServiceProvider serviceProvider)
		{
			var dbContext = serviceProvider.CreateScope().ServiceProvider.GetService<EmployeeDbContext>();
			dbContext.Database.Migrate();
		}
	}
}
