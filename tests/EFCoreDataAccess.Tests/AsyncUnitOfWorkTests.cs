using EFCoreDataAccess.Data;
using EFCoreDataAccess.Interfaces;
using EFCoreDataAccess.Models;
using EFCoreDataAccess.Tests.Infra;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;

namespace EFCoreDataAccess.Tests
{
    [Collection("Database collection")]
    public class AsyncUnitOfWorkTests
    {
        private readonly DatabaseFixture _databaseFixture;

        public AsyncUnitOfWorkTests(DatabaseFixture databaseFixture)
        {
            _databaseFixture = databaseFixture;
        }

        [Fact]
        public async Task BeginTransactionAsync_ThrowAnException_TryCreateMultiplesTransactions()
        {
            // Arrange
            using var scope = _databaseFixture.ServiceProvider.CreateScope();
            var uow = scope.ServiceProvider.GetService<IUnitOfWork<EmployeeDbContext>>();

            await uow.BeginTransactionAsync();

            // Act
            Func<Task> act = () => uow.BeginTransactionAsync();

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("There is already an active transaction");
        }

        [Fact]
        public async Task CommitAsync_ThrowAnException_NullTransaction()
        {
            // Arrange
            using var scope = _databaseFixture.ServiceProvider.CreateScope();
            var uow = scope.ServiceProvider.GetService<IUnitOfWork<EmployeeDbContext>>();

            // Act
            Func<Task> act = () => uow.CommitAsync();

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>()
                .WithMessage("Value cannot be null. (Parameter 'Transaction')");
        }

        [Fact]
        public async Task CommitAsync_CommitChangesSuccessful_BunchOfChanges()
        {
            // Arrange
            using var scope = _databaseFixture.ServiceProvider.CreateScope();
            var uow = scope.ServiceProvider.GetService<IUnitOfWork<EmployeeDbContext>>();

            await uow.BeginTransactionAsync();

            var companyRepository = uow.GetGenericRepository<Company>();
            var employeeRepository = uow.GetGenericRepository<Employee>();

            var disneyCompany = new Company(name: "Disney");
            var disneyEmployee = new Employee(
                name: "Peter P.",
                code: "003",
                position: "Actor",
                birthDate: new DateTime(1996, 06, 01));

            disneyCompany = await companyRepository.AddAsync(disneyCompany);
            await uow.SaveChangesAsync();

            disneyEmployee.SetCompany(disneyCompany.Id);
            employeeRepository.Add(disneyEmployee);
            await uow.SaveChangesAsync();

            // Act
            await uow.CommitAsync();

            // Assert
            disneyCompany.Id.Should().BeGreaterThan(0);
            disneyEmployee.Id.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task RollbackAsync_RollbackChangesSuccessful_BunchOfChanges()
        {
            // Arrange
            using var scope = _databaseFixture.ServiceProvider.CreateScope();
            var uow = scope.ServiceProvider.GetService<IUnitOfWork<EmployeeDbContext>>();

            await uow.BeginTransactionAsync();

            var companyRepository = uow.GetGenericRepository<Company>();
            var employeeRepository = uow.GetGenericRepository<Employee>();

            var disneyCompany = new Company(name: "Disney");
            var disneyEmployee = new Employee(
                name: "Peter P.",
                code: "003",
                position: "Actor",
                birthDate: new DateTime(1996, 06, 01));

            disneyCompany = await companyRepository.AddAsync(disneyCompany);
            await uow.SaveChangesAsync();

            disneyEmployee.SetCompany(disneyCompany.Id);
            await employeeRepository.AddAsync(disneyEmployee);
            await uow.SaveChangesAsync();

            // Act
            await uow.RollbackAsync();

            // Assert
            companyRepository.FirstOrDefault(o => o.Id == disneyCompany.Id).Should().BeNull();
            employeeRepository.FirstOrDefault(o => o.Id == disneyEmployee.Id).Should().BeNull();
        }
    }
}
