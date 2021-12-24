using EFCoreDataAccess.Data;
using EFCoreDataAccess.Interfaces;
using EFCoreDataAccess.Models;
using EFCoreDataAccess.Tests.Infra;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
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
            var companiesStored = await companyRepository.SearchAsync(c => listOfCompanies.Select(o => o.Id).Contains(c.Id));
            listOfCompanies.Should().BeEquivalentTo(companiesStored);
        }

        [Fact]
        public void Any_ShouldReturnTrue_GivenAnExistingId()
        {
            // Arrange
            using var scope = _databaseFixture.ServiceProvider.CreateScope();
            var uow = scope.ServiceProvider.GetService<IUnitOfWork<EmployeeDbContext>>();

            var companyRepository = uow.GetGenericRepository<Company>();

            var entityId = 1;

            // Act
            var result = companyRepository.Any(c => c.Id == entityId);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task AnyAsync_ShouldReturnFalse_GivenAnInvalidId()
        {
            // Arrange
            using var scope = _databaseFixture.ServiceProvider.CreateScope();
            var uow = scope.ServiceProvider.GetService<IUnitOfWork<EmployeeDbContext>>();

            var companyRepository = uow.GetGenericRepository<Company>();

            var entityId = long.MaxValue;

            // Act
            var result = await companyRepository.AnyAsync(c => c.Id == entityId);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void Count_ShouldReturnTheTotalOfEntities_GivenTableWithThreeRows()
        {
            // Arrange
            using var scope = _databaseFixture.ServiceProvider.CreateScope();
            var uow = scope.ServiceProvider.GetService<IUnitOfWork<EmployeeDbContext>>();

            var companyRepository = uow.GetGenericRepository<Company>();

            var entityId = 1;
            var expectedCountRows = 1;

            // Act
            var countRows = companyRepository.Count();
            var countRowsWhere = companyRepository.Count(c => c.Id == entityId);

            // Assert
            countRows.Should().BeGreaterThan(0);
            countRowsWhere.Should().Be(expectedCountRows);
        }

        [Fact]
        public async Task CountAsync_ShouldReturnTheTotalOfEntities_GivenTableWithThreeRows()
        {
            // Arrange
            using var scope = _databaseFixture.ServiceProvider.CreateScope();
            var uow = scope.ServiceProvider.GetService<IUnitOfWork<EmployeeDbContext>>();

            var companyRepository = uow.GetGenericRepository<Company>();

            var entityId = 1;
            var expectedCountRows = 1;

            // Act
            var countRows = await companyRepository.CountAsync();
            var countRowsWhere = await companyRepository.CountAsync(c => c.Id == entityId);

            // Assert
            countRows.Should().BeGreaterThan(0);
            countRowsWhere.Should().Be(expectedCountRows);
        }

        [Fact]
        public async Task FirstOrDefaultAsync_ShouldReturnTheFirstEntity_GivenTableWithEntities()
        {
            // Arrange
            using var scope = _databaseFixture.ServiceProvider.CreateScope();
            var uow = scope.ServiceProvider.GetService<IUnitOfWork<EmployeeDbContext>>();

            var companyRepository = uow.GetGenericRepository<Company>();

            // Act
            var result = await companyRepository.FirstOrDefaultAsync(c => c.Id > 0);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
        }

        [Fact]
        public async Task SingleOrDefaultAsync_ShouldReturnAnEntity_GivenValidEntityId()
        {
            // Arrange
            using var scope = _databaseFixture.ServiceProvider.CreateScope();
            var uow = scope.ServiceProvider.GetService<IUnitOfWork<EmployeeDbContext>>();

            var companyRepository = uow.GetGenericRepository<Company>();
            var validEntity = 1;
            var invalidEntity = long.MaxValue;

            // Act
            var entity = await companyRepository.SingleOrDefaultAsync(c => c.Id == validEntity);
            var nullEntity = await companyRepository.SingleOrDefaultAsync(c => c.Id == invalidEntity);

            // Assert
            entity.Should().NotBeNull();
            nullEntity.Should().BeNull();
        }

        [Fact]
        public void SingleOrDefault_ShouldReturnAnEntity_GivenValidEntityId()
        {
            // Arrange
            using var scope = _databaseFixture.ServiceProvider.CreateScope();
            var uow = scope.ServiceProvider.GetService<IUnitOfWork<EmployeeDbContext>>();

            var companyRepository = uow.GetGenericRepository<Company>();
            var validEntity = 1;
            var invalidEntity = long.MaxValue;

            // Act
            var entity = companyRepository.SingleOrDefault(c => c.Id == validEntity);
            var nullEntity = companyRepository.SingleOrDefault(c => c.Id == invalidEntity);

            // Assert
            entity.Should().NotBeNull();
            nullEntity.Should().BeNull();
        }

        [Fact]
        public void Dispose_ShouldDispose_GivenValidGenericRepositoryInstance()
        {
            // Arrange
            using var scope = _databaseFixture.ServiceProvider.CreateScope();
            var uow = scope.ServiceProvider.GetService<IUnitOfWork<EmployeeDbContext>>();

            var companyRepository = uow.GetGenericRepository<Company>();

            Action act = () => companyRepository.Count();

            // Act
            companyRepository.Dispose();

            // Assert
            act.Should().Throw<ObjectDisposedException>();
        }
    }
}
