using System.Reflection;
using Azure.Messaging.ServiceBus;
using Daya.Sample.Infrastructure.Configuration.CosmosDatabase;
using Daya.Sample.IntegrationTests.Configuration;
using Daya.Sample.IntegrationTests.Logging;
using DAYA.Cloud.Framework.V2.Application.Contracts;
using DAYA.Cloud.Framework.V2.Domain;
using DAYA.Cloud.Framework.V2.Infrastructure.AzureServiceBus;
using DAYA.Cloud.Framework.V2.Infrastructure.Configuration;
using DAYA.Cloud.Framework.V2.Infrastructure.EventBus;
using DAYA.Cloud.Framework.V2.ServiceBus;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit.Abstractions;

namespace Daya.Sample.IntegrationTests.SeedWork
{
    public class TestFixture : IDisposable
    {
        private const LogLevel _logLevel = LogLevel.Trace;
        public static ITestOutputHelper? Output { get; set; }
        public static IServiceModule ServiceModule { get; private set; } = null!;
        public ITopicClientFactory? TopicClientFactory;
        public IQueueClientFactory? QueueClientFactory;
        public IEventBus? EventBus;

        private readonly IServiceCollection _services;
        private readonly IServiceProvider _serviceProvider;

        private readonly Assembly _infrastructureAssembly = Assembly.Load("Daya.Sample.Infrastructure");
        private readonly Assembly _applicationAssembly = Assembly.Load("Daya.Sample.Application");

        private readonly string _databaseId = "TestDB_" + Guid.NewGuid().ToString().Substring(0, 6);

        internal Guid TenantId => FakeAccessor.Instance.TenantId.Value;
        internal Guid UserId => FakeAccessor.Instance.UserId;
        public IServiceProvider ServiceProvider => _serviceProvider;

        public TestFixture()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.test.json", true)
                .AddEnvironmentVariables()
                .Build();

            _services = new ServiceCollection();
            _services.AddSingleton<IConfiguration>(configuration);

            _services.AddIntegrationTestLogging();

            _services.AddSingleton<IContextAccessor>(FakeAccessor.Instance);

            // Register your services here
            ConfigureServices(_services, configuration);
            _services.RegisterDomainServices();

            // Build the service provider
            _serviceProvider = _services.BuildServiceProvider();
            ServiceCompositionRoot.SetServiceProvider(_serviceProvider);

            // Initialize the service module
            ServiceModule = _serviceProvider.GetRequiredService<IServiceModule>();
        }

        private void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(Substitute.For<ServiceBusClient>());

            services.AddDayaMediator(configuration, _infrastructureAssembly, _applicationAssembly);
            services.AddSingleton<IDatabaseConfigurrator, DatabaseConfigurator>();

            services.AddIntegrationTestCosmosDb(configuration, _databaseId);

            services.AddSingleton<Database>(sp =>
            {
                var cosmosClient = sp.GetRequiredService<CosmosClient>();
                var database = cosmosClient.GetDatabase(_databaseId);
                return database;
            });
        }

        internal async Task InitializeTestAsync()
        {
            FakeAccessor.ResetTenantId();
            FakeAccessor.ResetUserId();
            Clock.Reset();

            var databaseConfigurrator = _serviceProvider.GetRequiredService<IDatabaseConfigurrator>();
            await databaseConfigurrator.ExecuteAsync(_databaseId);
        }

        internal void SetTenantId(Guid tenantId)
        {
            FakeAccessor.SetTenantId(tenantId);
        }

        public void Dispose()
        {
            DeleteDatabase().Wait();

            // Dispose resources, like service provider if needed
            (_serviceProvider as IDisposable)?.Dispose();
        }

        private async Task DeleteDatabase()
        {
            var database = _serviceProvider.GetRequiredService<Database>();
            try
            {
                await database.DeleteAsync();
            }
            catch
            {
                // Ignore exceptions during deletion, as the database might not exist
            }
        }
    }
}