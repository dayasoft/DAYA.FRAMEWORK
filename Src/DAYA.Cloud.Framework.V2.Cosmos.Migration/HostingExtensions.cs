using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetArchTest.Rules;

namespace DAYA.Cloud.Framework.V2.Cosmos.Migration;

public static class HostingExtensions
{
    public static WebApplicationBuilder ConfigureHost(this WebApplicationBuilder builder, string[]? args)
    {
        builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
        {
            config.Sources.Clear();
            var env = hostingContext.HostingEnvironment;
            config.AddJsonFile("appsettings.json", false);
            if (env.IsDevelopment()) config.AddJsonFile("appsettings.Development.json", true);
            config.AddEnvironmentVariables();
            if (args != null) config.AddCommandLine(args);
        });

        return builder;
    }

    public static IServiceCollection ConfigureServices(
        this WebApplicationBuilder builder
        , out IConfiguration configuration
        , out ILoggerFactory loggerFactory)
    {
        var services = new ServiceCollection();
        configuration = builder.Configuration;
        services.AddSingleton<IConfiguration>(_ => builder.Configuration);
        services.AddLogging(logBuilder => { logBuilder.AddConsole(); });
        loggerFactory = services.BuildServiceProvider().GetRequiredService<ILoggerFactory>();
        services.AddSingleton(loggerFactory.CreateLogger("migrator"));

        services.AddSingleton(_ =>
        {
            var serviceDatabaseConfig = new ServiceDatabaseConfig();
            builder.Configuration.GetSection(ServiceDatabaseConfig.ServiceDatabase).Bind(serviceDatabaseConfig);
            return serviceDatabaseConfig;
        });

        var migratorTypes = Types.InCurrentDomain()
            .That()
            .Inherit(typeof(Migrator))
            .GetTypes();

        foreach (var migratorType in migratorTypes) services.AddSingleton(typeof(Migrator), migratorType);

        return services;
    }

    public static ServiceProvider Build(this IServiceCollection services)
    {
        return services.BuildServiceProvider();
    }

    public static async Task RunMigrationsAsync(this ServiceProvider provider, Func<Migrator, bool>? checkFilterAction = null)
    {
        var migrators = provider.GetService<IEnumerable<Migrator>>();
        var logger = provider.GetService<ILogger>()!;

        var orderedMigrators = migrators!
            .OrderBy(x => x.Version)
            .ThenBy(x => x.Order);

        foreach (var migrator in orderedMigrators)
        {
            var migrationTitle = $"{migrator.GetType().Name}:({migrator.Version})";
            if (IsIgnored(migrator))
            {
                logger.LogInformation($"Migration {migrationTitle} ignored.");
                continue;
            }

            logger.LogInformation($"Migration {migrationTitle} started.");

            try
            {
                if (checkFilterAction is null || checkFilterAction(migrator))
                {
                    await migrator.MigrateAsync();
                    logger.LogInformation($"Migration {migrationTitle} executed successfully.");
                }
                else
                {
                    logger.LogError($"Migration {migrationTitle} skipped by checking filter.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Migration {migrationTitle} failed. Exception: " + ex);
                throw;
            }
        }
    }

    private static bool IsIgnored(Migrator migrator)
    {
        var attribute = migrator.GetType()
            .GetCustomAttributes(typeof(IgnoreAttribute), true)
            .FirstOrDefault();

        return attribute is not null;
    }
}