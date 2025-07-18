using Azure.Identity;
using DAYA.Cloud.Framework.V2.Cosmos;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Daya.Sample.WebAPI.Configuration.CosmosDatabase
{
    internal static class CosmosServiceCollectionExtension
    {
        public static IServiceCollection AddCosmosDb(this IServiceCollection services, IConfiguration configuration, DefaultAzureCredential? credential = null)
        {
            var databaseName = configuration.GetValue<string>("ServiceDatabaseConfig:DatabaseName");
            var accountEndpoint = configuration.GetValue<string>("ServiceDatabaseConfig:AccountEndpoint");
            var accountKey = configuration.GetValue<string>("ServiceDatabaseConfig:AccountKey");

            var jsonSerializerSetting = new JsonSerializerSettings
            {
                ContractResolver = new PrivateSetterContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                },
                Formatting = Formatting.Indented
            };

            var cosmosClientOptions = new CosmosClientOptions
            {
                Serializer = new CosmosJsonDotNetSerializer(jsonSerializerSetting)
            };

            CosmosClient cosmosClient;
            if (credential is not null)
            {
                cosmosClient = new CosmosClient(accountEndpoint, credential, cosmosClientOptions);
            }
            else
            {
                cosmosClient = new CosmosClient(accountEndpoint, accountKey, cosmosClientOptions);
            }

            services.AddSingleton<Database>(sp =>
            {
                //var configuration = sp.GetRequiredService<IConfiguration>();
                //var cosmosClient = sp.GetRequiredService<CosmosClient>();

                var database = cosmosClient.GetDatabase(databaseName);

                // Optionally, you can test the connection here synchronously, but ReadAsync is
                // async. For now, just return the database instance.
                return database;
            });

            return services;
        }
    }
}