using EFCoreDataAccess.Data;
using EFCoreDataAccess.Data.Repositories;
using EFCoreDataAccess.Models;
using EFCoreDataAccess.Models.Interface;
using EFCoreDataAccess.Tests.Infra;
using EFCoreUnitOfWork.Interfaces;
using EFCoreUnitOfWork.Repository;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;

namespace EFCoreDataAccess.Tests
{
	[Collection("Database collection")]
    public class UnitOfWorkTests
    {
        private readonly DatabaseFixture _databaseFixture;

        public UnitOfWorkTests(DatabaseFixture databaseFixture)
        {
            _databaseFixture = databaseFixture;
        }

        [Fact]
        public void GetGenericRepository_ShouldReturnConcreateGenericRepository_GivenAnEntity()
        {
            // Arrange
            var uow = _databaseFixture.ServiceProvider.GetService<IUnitOfWork<EmployeeDbContext>>();

            // Act
            var genericRepository = uow.GetGenericRepository<Employee>();

            // Assert
            Assert.IsAssignableFrom<GenericRepository<Employee>>(genericRepository);
        }

        [Fact]
        public void GetRepository_ShouldReturnConcreateCustomRepository_GivenConcreteRepositoryClass()
        {
            // Arrange
            var uow = _databaseFixture.ServiceProvider.GetService<IUnitOfWork<EmployeeDbContext>>();

            // Act
            var genericRepository = uow.GetRepository<AddressRepository>();

            // Assert
            Assert.IsAssignableFrom<AddressRepository>(genericRepository);
        }

        [Fact]
        public void GetRepository_ShouldReturnAnException_GivenAnInterface()
        {
            // Arrange
            var uow = _databaseFixture.ServiceProvider.GetService<IUnitOfWork<EmployeeDbContext>>();

            // Act
            Action act = () => uow.GetRepository<IAddressRepository>();

            // Assert
            act.Should()
                .Throw<InvalidOperationException>()
                .WithMessage("The type TRepository should be a class");
        }

        [Fact]
        public void GetRepository_ShouldReturnAnException_GivenAnAbstractRepository()
        {
            // Arrange
            var uow = _databaseFixture.ServiceProvider.GetService<IUnitOfWork<EmployeeDbContext>>();

            // Act
            Action act = () => uow.GetRepository<AbstractRepository>();

            // Assert
            act.Should()
                .Throw<InvalidOperationException>()
                .WithMessage("Could not create an object from an abstract class");
        }

        [Fact]
        public void Dispose_ShouldDispose_GivenValidUnitOfWorkInstance()
        {
            // Arrange
            var scope = _databaseFixture.ServiceProvider.CreateScope();
            var uow = scope.ServiceProvider.GetService<IUnitOfWork<EmployeeDbContext>>();

            // Act
            uow.Dispose();

            // Assert
            using (new AssertionScope())
            {
                uow.DbContext.Should().BeNull();
                uow.Transaction.Should().BeNull();
            }
        }
    }
}
