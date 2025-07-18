using System.Reflection;
using Daya.Sample.Domain.Commons;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

namespace Daya.Sample.Infrastructure.Configuration.CosmosDatabase
{
    public class DatabaseConfigurator : IDatabaseConfigurrator
    {
        private readonly CosmosClient _cosmosClient;
        private readonly IConfiguration _configuration;

        public DatabaseConfigurator(CosmosClient cosmosClient, IConfiguration configuration)
        {
            _cosmosClient = cosmosClient;
            _configuration = configuration;
        }

        public async Task ExecuteAsync(CancellationToken cancelationToken)
        {
            var databaseName = _configuration.GetValue<string>("ServiceDatabaseConfig:DatabaseName");

            await ExecuteAsync(databaseName!);
        }

        public async Task ExecuteAsync(string databaseName)
        {
            if (string.IsNullOrEmpty(databaseName))
            {
                throw new InvalidOperationException("Database name is not configured in ServiceDatabaseConfig:DatabaseName");
            }

            Console.WriteLine($"Starting database reset process for '{databaseName}'...");

            // First delete the database if it exists
            await ResetDatabase(databaseName);

            // Then create the database and containers
            await GenerateContainers(databaseName);

            Console.WriteLine("Database reset process completed successfully.");
        }

        private async Task GenerateContainers(string databaseName)
        {
            try
            {
                Console.WriteLine($"Creating database '{databaseName}' if it doesn't exist...");
                var databaseResponse = await _cosmosClient.CreateDatabaseIfNotExistsAsync(databaseName);
                Console.WriteLine($"Database '{databaseName}' ready.");

                var serviceDatabase = _cosmosClient.GetDatabase(databaseName);
                var containers = GetContainerNames();

                foreach (var containerName in containers)
                {
                    if (string.IsNullOrWhiteSpace(containerName) == false)
                    {
                        var containerProperties = new ContainerProperties(containerName, "/partitionKey");
                        await serviceDatabase.CreateContainerIfNotExistsAsync(containerProperties);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to create database: {ex.Message}");
                throw;
            }
        }

        private async Task ResetDatabase(string databaseName)
        {
            try
            {
                Console.WriteLine($"Attempting to delete database '{databaseName}'...");

                var database = _cosmosClient.GetDatabase(databaseName);
                await database.DeleteAsync();

                Console.WriteLine($"Database '{databaseName}' successfully deleted.");
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                Console.WriteLine($"Database '{databaseName}' does not exist. No deletion needed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to delete database: {ex.Message}");
            }
        }

        private IEnumerable<string?> GetContainerNames()
        {
            var containerFields = typeof(ServiceDatabaseContainers)
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(field => field is { IsLiteral: true, IsInitOnly: false } && field.FieldType == typeof(string))
                .ToList();

            return containerFields
                .Select(field => (string?)field.GetValue(null))
                .ToList();
        }
    }
}