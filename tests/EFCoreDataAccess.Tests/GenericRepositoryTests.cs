using EFCoreDataAccess.Data;
using EFCoreDataAccess.Interfaces;
using EFCoreDataAccess.Models;
using EFCoreDataAccess.Tests.Infra;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace EFCoreDataAccess.Tests
{
    [Collection("Database collection")]
    public class GenericRepositoryTests
    {
        private readonly DatabaseFixture _databaseFixture;

        public GenericRepositoryTests(DatabaseFixture databaseFixture)
        {
            _databaseFixture = databaseFixture;
        }

        [Fact]
        public void AddRange_SaveListOfEntitiesSuccessful_GivenListOfEntities()
        {
            // Arrange
            using var scope = _databaseFixture.ServiceProvider.CreateScope();
            var uow = scope.ServiceProvider.GetService<IUnitOfWork<EmployeeDbContext>>();

            var companyRepository = uow.GetGenericRepository<Company>();

            var listOfCompanies = new[]
            {
                new Company(name: "New York Times"),
                new Company(name: "BBC"),
                new Company(name: "Fox"),
            };

            // Act
            companyRepository.AddRange(listOfCompanies);
            uow.SaveChanges();

            // Assert
            var companiesStored = companyRepository.Search(c => listOfCompanies.Select(o => o.Id).Contains(c.Id));
            listOfCompanies.Should().BeEquivalentTo(companiesStored);
        }

        [Fact]
        public async Task AddRangeAsync_SaveListOfEntitiesSuccessful_GivenListOfEntities()
        {
            // Arrange
            using var scope = _databaseFixture.ServiceProvider.CreateScope();
            var uow = scope.ServiceProvider.GetService<IUnitOfWork<EmployeeDbContext>>();

            var companyRepository = uow.GetGenericRepository<Company>();

            var listOfCompanies = new[]
            {
                new Company(name: "Netflix"),
                new Company(name: "Prime Videos"),
                new Company(name: "HBO Max"),
            };

            // Act
            await companyRepository.AddRangeAsync(listOfCompanies);
            await uow.SaveChangesAsync();

            // Assert
        }
    }
}
