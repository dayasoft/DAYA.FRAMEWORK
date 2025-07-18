namespace Daya.Sample.Infrastructure.Configuration.CosmosDatabase
{
    public record ServiceDatabaseConfig(string AccountEndpoint, string AccountKey, string DatabaseName);
}