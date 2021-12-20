﻿using EFCoreDataAccess.Data;
using EFCoreDataAccess.Data.Repositories;
using EFCoreDataAccess.Interfaces;
using EFCoreDataAccess.Models;
using EFCoreDataAccess.Models.Interface;
using EFCoreDataAccess.Repository;
using EFCoreDataAccess.Tests.Infra;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;

namespace EFCoreDataAccess.Tests
{
    [CollectionDefinition("Database collection")]
    public class UnitOfWorkTests : IClassFixture<DatabaseFixture>
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
    }
}
