using Azure.Identity;
using DAYA.Cloud.Framework.V2.Cosmos;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Daya.Sample.Infrastructure.Configuration.CosmosDatabase
{
    public static class CosmosServiceCollectionExtension
    {
        /// <summary>
        /// Adds Cosmos DB services to the service collection with configuration via Action delegate
        /// </summary>
        public static IServiceCollection AddCosmosDb(
            this IServiceCollection services,
            Func<ServiceDatabaseConfig, ServiceDatabaseConfig> configureOptions = null,
            CosmosClientOptions? clientOptions = null,
            DefaultAzureCredential? credential = null)
        {
            CosmosClient cosmosClient;

            // Create the config instance with initial empty values
            var config = new ServiceDatabaseConfig(string.Empty, string.Empty, string.Empty);

            // Apply configuration function if provided
            if (configureOptions != null)
            {
                config = configureOptions(config);
            }

            // Set default options if not provided
            var cosmosClientOptions = clientOptions ?? new CosmosClientOptions
            {
                Serializer = new CosmosJsonDotNetSerializer(new JsonSerializerSettings
                {
                    ContractResolver = new PrivateSetterContractResolver
                    {
                        NamingStrategy = new CamelCaseNamingStrategy()
                    },
                    Formatting = Formatting.Indented
                })
            };

            // Create client with either credential or account key
            if (credential != null)
            {
                cosmosClient = new CosmosClient(config.AccountEndpoint, credential, cosmosClientOptions);
            }
            else
            {
                cosmosClient = new CosmosClient(config.AccountEndpoint, config.AccountKey, cosmosClientOptions);
            }

            services.AddSingleton(cosmosClient);

            return services;
        }

        /// <summary>
        /// Adds Cosmos DB services to the service collection with direct parameters
        /// </summary>
        public static IServiceCollection AddCosmosDb(
            this IServiceCollection services,
            string databaseName,
            string accountEndpoint,
            string accountKey,
            CosmosClientOptions? cosmosClientOptions = null,
            DefaultAzureCredential? credential = null)
        {
            // Use a Func that creates and returns the new configuration
            return AddCosmosDb(
                services,
                _ => new ServiceDatabaseConfig(
                    accountEndpoint ?? string.Empty,
                    accountKey ?? string.Empty,
                    databaseName ?? string.Empty
                ),
                cosmosClientOptions,
                credential);
        }
    }
}