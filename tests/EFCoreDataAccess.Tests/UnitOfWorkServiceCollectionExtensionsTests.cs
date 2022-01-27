using EFCoreDataAccess.Data;
using EFCoreUnitOfWork.Extensions;
using EFCoreUnitOfWork.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Xunit;

namespace EFCoreDataAccess.Tests
{
	public class UnitOfWorkServiceCollectionExtensionsTests
    {
        [Fact]
        public void AddUnitOfWork_ShouldThrowNullException_NullableServiceCollection()
        {
            // Arrange
            IServiceCollection services = null;

            // Act
            Action act = () => services.AddUnitOfWork<EmployeeDbContext>();

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithMessage("Value cannot be null. (Parameter 'services')");
        }

        [Fact]
        public void AddUnitOfWork_ShouldRegisterAsSingleton_ChooseSingletonLifetime()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();

            // Act
            services.AddUnitOfWork<EmployeeDbContext>(ServiceLifetime.Singleton);

            // Assert
            var service = services.FirstOrDefault(s => s.ServiceType == typeof(IUnitOfWork<EmployeeDbContext>));
            service.Lifetime.Should().Be(ServiceLifetime.Singleton);
        }

        [Fact]
        public void AddUnitOfWork_ShouldRegisterAsTransient_ChooseTransientLifetime()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();

            // Act
            services.AddUnitOfWork<EmployeeDbContext>(ServiceLifetime.Transient);

            // Assert
            var service = services.FirstOrDefault(s => s.ServiceType == typeof(IUnitOfWork<EmployeeDbContext>));
            service.Lifetime.Should().Be(ServiceLifetime.Transient);
        }

        [Fact]
        public void AddUnitOfWork_ShouldRegisterAsScoped_ChooseScopedLifetime()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();

            // Act
            services.AddUnitOfWork<EmployeeDbContext>(ServiceLifetime.Scoped);

            // Assert
            var service = services.FirstOrDefault(s => s.ServiceType == typeof(IUnitOfWork<EmployeeDbContext>));
            service.Lifetime.Should().Be(ServiceLifetime.Scoped);
        }
    }
}
