using EFCoreDataAccess.Data;
using EFCoreDataAccess.Interfaces;
using EFCoreDataAccess.Models;
using EFCoreDataAccess.Tests.Infra;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;

namespace EFCoreDataAccess.Tests
{
    [Collection("Database collection")]
    public class SyncUnitOfWork 
    {
        private readonly DatabaseFixture _databaseFixture;

        public SyncUnitOfWork(DatabaseFixture databaseFixture)
        {
            _databaseFixture = databaseFixture;
        }

        [Fact]
        public void BeginTransaction_ThrowAnException_TryCreateMultiplesTransactions()
        {
            // Arrange
            using var scope = _databaseFixture.ServiceProvider.CreateScope();
            var uow = scope.ServiceProvider.GetService<IUnitOfWork<EmployeeDbContext>>();

            uow.BeginTransaction();

            // Act
            Action act = () => uow.BeginTransaction();

            // Assert
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("There is already an active transaction");
        }

        [Fact]
        public void Commit_ThrowAnException_NullTransaction()
        {
            // Arrange
            using var scope = _databaseFixture.ServiceProvider.CreateScope();
            var uow = scope.ServiceProvider.GetService<IUnitOfWork<EmployeeDbContext>>();

            // Act
            Action act = () => uow.Commit();

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithMessage("Value cannot be null. (Parameter 'Transaction')");
        }

        [Fact]
        public void Commit_CommitChangesSuccessful_BunchOfChanges()
        {
            // Arrange
            using var scope = _databaseFixture.ServiceProvider.CreateScope();
            var uow = scope.ServiceProvider.GetService<IUnitOfWork<EmployeeDbContext>>();

            uow.BeginTransaction();

            var addressRepository = uow.GetGenericRepository<Address>();
            var companyRepository = uow.GetGenericRepository<Company>();

            var address = new Address(
                street: "2, Pink",
                city: "Nowhere",
                state: "Nowhere",
                country: "Nowhere",
                postalCode: "132456");

            addressRepository.Add(address);
            uow.SaveChanges();

            var disneyCompany = new Company(name: "Disney");
            disneyCompany.SetAddress(address.Id);

            disneyCompany = companyRepository.Add(disneyCompany);
            uow.SaveChanges();

            // Act
            uow.Commit();

            // Assert
            disneyCompany.Id.Should().BeGreaterThan(0);
            address.Id.Should().BeGreaterThan(0);
        }

        [Fact]
        public void Rollback_RollbackChangesSuccessful_BunchOfChanges()
        {
            // Arrange
            using var scope = _databaseFixture.ServiceProvider.CreateScope();
            var uow = scope.ServiceProvider.GetService<IUnitOfWork<EmployeeDbContext>>();

            uow.BeginTransaction();

            var addressRepository = uow.GetGenericRepository<Address>();
            var companyRepository = uow.GetGenericRepository<Company>();

            var address = new Address(
                street: "2, Pink",
                city: "Nowhere",
                state: "Nowhere",
                country: "Nowhere",
                postalCode: "132456");

            addressRepository.Add(address);
            uow.SaveChanges();

            var disneyCompany = new Company(name: "Disney");
            disneyCompany.SetAddress(address.Id);
            disneyCompany = companyRepository.Add(disneyCompany);

            uow.SaveChanges();

            // Act
            uow.Rollback();

            // Assert
            companyRepository.FirstOrDefault(o => o.Id == disneyCompany.Id).Should().BeNull();
            addressRepository.FirstOrDefault(o => o.Id == address.Id).Should().BeNull();
        }
    }
}
