using EFCoreDataAccess.Tests.Infra;
using Xunit;

namespace EFCoreDataAccess.Tests
{
    [CollectionDefinition("Database collection")]
    public class GenericRepositoryTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _databaseFixture;

        public GenericRepositoryTests(DatabaseFixture databaseFixture)
        {
            _databaseFixture = databaseFixture;
        }
    }
}
