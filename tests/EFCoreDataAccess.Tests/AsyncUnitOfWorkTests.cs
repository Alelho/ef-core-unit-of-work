using EFCoreDataAccess.Data;
using EFCoreDataAccess.Interfaces;
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
        public async Task BeginTransactionAsync_Throw_TryCreateMultiplesTransactions()
        {
            // Arrange
            var uow = _databaseFixture.ServiceProvider.GetService<IUnitOfWork<EmployeeDbContext>>();

            await uow.BeginTransactionAsync();

            // Act
            Func<Task> act = () => uow.BeginTransactionAsync();

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("There is already an active transaction");
        }
    }
}
