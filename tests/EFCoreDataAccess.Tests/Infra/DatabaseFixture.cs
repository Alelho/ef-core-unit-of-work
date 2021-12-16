using EFCoreDataAccess.Data;
using EFCoreDataAccess.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;

namespace EFCoreDataAccess.Tests.Infra
{
    public class DatabaseFixture : IDisposable
    {
        public DatabaseFixture()
        {
            var services = new ServiceCollection();

            var connectionString = @"Server=localhost;Database=DataAccessTests;Uid=root;Pwd=123456;";

            services.AddDbContext<EmployeeDbContext>(options =>
                options.UseMySql(connectionString, serverVersion: ServerVersion.AutoDetect(connectionString))
                .LogTo(msg => Debug.WriteLine(msg), LogLevel.Error));

            services.AddUnitOfWork<EmployeeDbContext>();

            ServiceProvider = services.BuildServiceProvider();

            SeedHelper.Populate(ServiceProvider);
        }

        public IServiceProvider ServiceProvider { get; private set; }

        #region IDisposable Members

        private bool _disposed;

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed || !disposing) return;

            var employeeContext = ServiceProvider.GetService<EmployeeDbContext>();

            employeeContext.Database.EnsureDeleted();
            employeeContext.Dispose();

            _disposed = true;
        }

        #endregion
    }
}
