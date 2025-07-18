using Daya.Sample.Infrastructure.Configuration.CosmosDatabase;
using Jobbiplace.SubscriptionService.DataReset;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

Console.WriteLine("Start data migration");

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        var env = context.HostingEnvironment;

        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        config.AddEnvironmentVariables();
        config.AddCommandLine(args);
    })
    .ConfigureServices((context, services) =>
    {
        var databaseName = context.Configuration.GetValue<string>("ServiceDatabaseConfig:DatabaseName");
        var accountEndpoint = context.Configuration.GetValue<string>("ServiceDatabaseConfig:AccountEndpoint");
        var accountKey = context.Configuration.GetValue<string>("ServiceDatabaseConfig:AccountKey");

        services.AddCosmosDb(databaseName!, accountEndpoint!, accountKey!, null);

        services.AddSingleton<IDatabaseConfigurrator, DatabaseConfigurator>();
        services.AddHostedService<SharedTaskCoordinator>();
    })
    .Build();

host.Run();