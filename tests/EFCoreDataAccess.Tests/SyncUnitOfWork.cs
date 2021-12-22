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

            var companyRepository = uow.GetGenericRepository<Company>();
            var employeeRepository = uow.GetGenericRepository<Employee>();

            var disneyCompany = new Company(name: "Disney");
            var disneyEmployee = new Employee(
                name: "Peter P.",
                code: "003",
                position: "Actor",
                birthDate: new DateTime(1996, 06, 01));

            disneyCompany = companyRepository.Add(disneyCompany);
            uow.SaveChanges();

            disneyEmployee.SetCompany(disneyCompany.Id);
            employeeRepository.Add(disneyEmployee);
            uow.SaveChanges();

            // Act
            uow.Commit();

            // Assert
            disneyCompany.Id.Should().BeGreaterThan(0);
            disneyEmployee.Id.Should().BeGreaterThan(0);
        }

        [Fact]
        public void Rollback_RollbackChangesSuccessful_BunchOfChanges()
        {
            // Arrange
            using var scope = _databaseFixture.ServiceProvider.CreateScope();
            var uow = scope.ServiceProvider.GetService<IUnitOfWork<EmployeeDbContext>>();

            uow.BeginTransaction();

            var companyRepository = uow.GetGenericRepository<Company>();
            var employeeRepository = uow.GetGenericRepository<Employee>();

            var disneyCompany = new Company(name: "Disney");
            var disneyEmployee = new Employee(
                name: "Peter P.",
                code: "003",
                position: "Actor",
                birthDate: new DateTime(1996, 06, 01));

            disneyCompany = companyRepository.Add(disneyCompany);
            uow.SaveChanges();

            disneyEmployee.SetCompany(disneyCompany.Id);
            employeeRepository.Add(disneyEmployee);
            uow.SaveChanges();

            // Act
            uow.Rollback();

            // Assert
            companyRepository.FirstOrDefault(o => o.Id == disneyCompany.Id).Should().BeNull();
            employeeRepository.FirstOrDefault(o => o.Id == disneyEmployee.Id).Should().BeNull();
        }
    }
}
