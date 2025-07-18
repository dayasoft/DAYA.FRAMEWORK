using Xunit;

namespace Daya.Sample.IntegrationTests.SeedWork
{
    [CollectionDefinition("Database collection")]
    public class DatabaseCollection : ICollectionFixture<TestFixture>
    {
        // This class has no code, it's just the way xUnit works
        // This class acts as a collection fixture definition to share a single
        // TestFixture instance between tests in the same collection
    }
}
