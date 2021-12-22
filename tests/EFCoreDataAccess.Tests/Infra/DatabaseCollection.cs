using Xunit;

namespace EFCoreDataAccess.Tests.Infra
{
    [CollectionDefinition("Database collection")]
    public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
    {
    }
}
