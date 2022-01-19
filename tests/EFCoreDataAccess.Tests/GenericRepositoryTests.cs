using EFCoreDataAccess.Builders;
using EFCoreDataAccess.Data;
using EFCoreDataAccess.Extensions;
using EFCoreDataAccess.Interfaces;
using EFCoreDataAccess.Models;
using EFCoreDataAccess.Tests.Infra;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
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

			var addressRepository = uow.GetGenericRepository<Address>();

			var listOfAddresses = new[]
			{
				new Address(
					street: "1, Street A",
					city: "Boston",
					state: "MA",
					country: "EUA",
					postalCode: "6543245"),
				new Address(
					street: "2, Street B",
					city: "Malibu",
					state: "CA",
					country: "EUA",
					postalCode: "98645"),
				new Address(
					street: "3, Street C",
					city: "El Paso",
					state: "TX",
					country: "EUA",
					postalCode: "12345")
			};

			// Act
			addressRepository.AddRange(listOfAddresses);
			uow.SaveChanges();

			// Assert
			var companiesStored = addressRepository.Search(c => listOfAddresses.Select(o => o.Id).Contains(c.Id));
			listOfAddresses.Should().BeEquivalentTo(companiesStored);
		}

		[Fact]
		public async Task AddRangeAsync_SaveListOfEntitiesSuccessful_GivenListOfEntities()
		{
			// Arrange
			using var scope = _databaseFixture.ServiceProvider.CreateScope();
			var uow = scope.ServiceProvider.GetService<IUnitOfWork<EmployeeDbContext>>();

			var addressRepository = uow.GetGenericRepository<Address>();

			var listOfAddresses = new[]
			{
				new Address(
					street: "1, Street A",
					city: "Boston",
					state: "MA",
					country: "EUA",
					postalCode: "6543245"),
				new Address(
					street: "2, Street B",
					city: "Malibu",
					state: "CA",
					country: "EUA",
					postalCode: "98645"),
				new Address(
					street: "3, Street C",
					city: "El Paso",
					state: "TX",
					country: "EUA",
					postalCode: "12345")
			};

			// Act
			await addressRepository.AddRangeAsync(listOfAddresses);
			await uow.SaveChangesAsync();

			// Assert
			var companiesStored = await addressRepository.SearchAsync(c => listOfAddresses.Select(o => o.Id).Contains(c.Id));
			listOfAddresses.Should().BeEquivalentTo(companiesStored);
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
		public void FirstOrDefault_ShouldReturnTheFirstEntityWithYourChildren_GivenTableWithEntities()
		{
			// Arrange
			using var scope = _databaseFixture.ServiceProvider.CreateScope();
			var uow = scope.ServiceProvider.GetService<IUnitOfWork<EmployeeDbContext>>();

			var companyRepository = uow.GetGenericRepository<Company>();

			var includeQuery = IncludeQuery<Company>.Builder()
				.Include(o => o.Include(o => o.Employees).ThenInclude(o => o.EmployeeEarnings))
				.Include(o => o.Include(o => o.Address));

			// Act
			var result = companyRepository.FirstOrDefault(c => c.Id > 0, includeQuery);

			// Assert
			using (new AssertionScope())
			{
				result.Address.Should().NotBeNull();
				result.Employees.Should().NotBeNullOrEmpty();
				result.Employees.Select(o => o.EmployeeEarnings).Should().NotBeNullOrEmpty();
			}
		}

		[Fact]
		public void LastOrDefault_ShouldReturnTheLastEntity_GivenTableWithEntities()
		{
			// Arrange
			using var scope = _databaseFixture.ServiceProvider.CreateScope();
			var uow = scope.ServiceProvider.GetService<IUnitOfWork<EmployeeDbContext>>();

			var companyRepository = uow.GetGenericRepository<Company>();

			// Act
			var result = companyRepository.LastOrDefault(c => c.Id > 0, c => c.Id);

			// Assert
			result.Should().NotBeNull();
			result.Id.Should().BeGreaterThan(1);
		}

		[Fact]
		public async Task LastOrDefaultAsync_ShouldReturnTheLastEntity_GivenTableWithEntities()
		{
			// Arrange
			using var scope = _databaseFixture.ServiceProvider.CreateScope();
			var uow = scope.ServiceProvider.GetService<IUnitOfWork<EmployeeDbContext>>();

			var companyRepository = uow.GetGenericRepository<Company>();

			// Act
			var result = await companyRepository.LastOrDefaultAsync(c => c.Id > 0, c => c.Id);

			// Assert
			result.Should().NotBeNull();
			result.Id.Should().BeGreaterThan(1);
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
		public void Update_ShouldUpdateTheEntity_GivenModifiedEntity()
		{
			// Arrange
			using var scope = _databaseFixture.ServiceProvider.CreateScope();
			var uow = scope.ServiceProvider.GetService<IUnitOfWork<EmployeeDbContext>>();

			var addressRepository = uow.GetGenericRepository<Address>();

			var address = new Address(
				street: "3, Bourrassol",
				city: "Boston",
				state: "CA",
				country: "France",
				postalCode: "11333");

			addressRepository.Add(address);
			uow.SaveChanges();

			address.EditAddress(
				street: address.Street,
				city: "Toulouse",
				state: "TO",
				country: address.Country,
				postalCode: "768900");

			// Act
			addressRepository.Update(address);
			uow.SaveChanges();

			// Assert
			var editedAddress = addressRepository.SingleOrDefault(a => a.Id == address.Id);

			address.Should().BeEquivalentTo(editedAddress);
		}

		[Fact]
		public void Update_ShouldUpdateOnlySelectedProperties_GivenModifiedProperties()
		{
			// Arrange
			using var scope = _databaseFixture.ServiceProvider.CreateScope();
			var uow = scope.ServiceProvider.GetService<IUnitOfWork<EmployeeDbContext>>();

			var addressRepository = uow.GetGenericRepository<Address>();

			var address = new Address(
				street: "1578, Paulista",
				city: "Rio de Janeiro",
				state: "RJ",
				country: "Brazil",
				postalCode: "99009900");

			addressRepository.Add(address);
			uow.SaveChanges();

			var newCity = "São Paulo";
			var newState = "SP";

			var expectedResult = new Address(address.Street, newCity, newState, address.Country, address.PostalCode);

			address.EditAddress(
				street: "",
				city: newCity,
				state: newState,
				country: "",
				postalCode: "");

			// Act
			addressRepository.Update(address, o => o.City, o => o.State);
			uow.SaveChanges();

			// Assert
			var modifiedAddress = addressRepository.SingleOrDefault(a => a.Id == address.Id);

			modifiedAddress.Should().BeEquivalentTo(expectedResult, opt => opt.Excluding(o => o.Id));
		}

		[Fact]
		public void UpdateRange_ShouldUpdateListOfModifiedEntities_GivenModifiedEntities()
		{
			// Arrange
			using var scope = _databaseFixture.ServiceProvider.CreateScope();
			var uow = scope.ServiceProvider.GetService<IUnitOfWork<EmployeeDbContext>>();

			var addressRepository = uow.GetGenericRepository<Address>();

			var copacabaAddress = new Address(
				street: "11, Copacabana",
				city: "Rio de Janeiro",
				state: "RJ",
				country: "Brazil",
				postalCode: "44009900");

			var stewartAddress = new Address(
				street: "111, Stewart",
				city: "Boston",
				state: "MA",
				country: "EUA",
				postalCode: "656565");

			addressRepository.AddRange(new[] { copacabaAddress, stewartAddress });
			uow.SaveChanges();

			copacabaAddress.EditAddress(
				copacabaAddress.Street,
				copacabaAddress.City,
				copacabaAddress.State,
				copacabaAddress.Country,
				postalCode: "12123344");

			stewartAddress.EditAddress(
				stewartAddress.Street,
				stewartAddress.City,
				stewartAddress.State,
				stewartAddress.Country,
				postalCode: "33344400");

			// Act
			addressRepository.UpdateRange(new[] { copacabaAddress, stewartAddress });
			uow.SaveChanges();

			// Assert
			var copacabaModifiedAddress = addressRepository.SingleOrDefault(a => a.Id == copacabaAddress.Id);
			var stewartModifiedAddress = addressRepository.SingleOrDefault(a => a.Id == stewartAddress.Id);

			copacabaModifiedAddress.Should().BeEquivalentTo(copacabaAddress);
			stewartModifiedAddress.Should().BeEquivalentTo(stewartAddress);
		}

		[Fact]
		public void RemoveByEntity_ShouldRemoveTheEntity_GivenAnEntity()
		{
			// Arrange
			using var scope = _databaseFixture.ServiceProvider.CreateScope();
			var uow = scope.ServiceProvider.GetService<IUnitOfWork<EmployeeDbContext>>();

			var addressRepository = uow.GetGenericRepository<Address>();

			var addressToRemove = new Address(
				street: "-1, Invalid address",
				city: "-",
				state: "-",
				country: "-",
				postalCode: "3333333");

			addressRepository.Add(addressToRemove);
			uow.SaveChanges();

			var addressToRemoveId = addressToRemove.Id;

			// Act
			addressRepository.RemoveByEntity(addressToRemove);
			uow.SaveChanges();

			// Assert
			var removedAddress = addressRepository.SingleOrDefault(a => a.Id == addressToRemoveId);

			removedAddress.Should().BeNull();
		}

		[Fact]
		public void RemoveSingle_ShouldRemoveTheEntity_GivenAnEntity()
		{
			// Arrange
			using var scope = _databaseFixture.ServiceProvider.CreateScope();
			var uow = scope.ServiceProvider.GetService<IUnitOfWork<EmployeeDbContext>>();

			var addressRepository = uow.GetGenericRepository<Address>();

			var addressToRemove = new Address(
				street: "-1, Invalid address",
				city: "-",
				state: "-",
				country: "-",
				postalCode: "3333333");

			addressRepository.Add(addressToRemove);
			uow.SaveChanges();

			var addressToRemoveId = addressToRemove.Id;

			// Act
			addressRepository.RemoveSingle(a => a.Id == addressToRemoveId);
			uow.SaveChanges();

			// Assert
			var removedAddress = addressRepository.SingleOrDefault(a => a.Id == addressToRemoveId);

			removedAddress.Should().BeNull();
		}

		[Fact]
		public void RemoveRange_ShouldRemoveListOfEntities_GivenEntityListToRemove()
		{
			// Arrange
			using var scope = _databaseFixture.ServiceProvider.CreateScope();
			var uow = scope.ServiceProvider.GetService<IUnitOfWork<EmployeeDbContext>>();

			var addressRepository = uow.GetGenericRepository<Address>();

			var addressToRemove1 = new Address(
				street: "-1, Invalid address",
				city: "-",
				state: "-",
				country: "-",
				postalCode: "3333333");

			var addressToRemove2 = new Address(
				street: "-2, Invalid address",
				city: "-",
				state: "-",
				country: "-",
				postalCode: "4444444");

			addressRepository.AddRange(new[] { addressToRemove1, addressToRemove2 });
			uow.SaveChanges();

			var addressToRemoveIds = new[] { addressToRemove1.Id, addressToRemove2.Id };

			// Act
			addressRepository.RemoveRange(new[] { addressToRemove1, addressToRemove2 });
			uow.SaveChanges();

			// Assert
			var removedAddresses = addressRepository.Search(a => addressToRemoveIds.Contains(a.Id));

			removedAddresses.Should().BeEmpty();
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
