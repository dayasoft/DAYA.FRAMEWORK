using System.Linq;
using System.Reflection;
using DAYA.Cloud.Framework.V2.Application.Configuration.Commands;
using DAYA.Cloud.Framework.V2.Application.Contracts;
using DAYA.Cloud.Framework.V2.Application.InternalCommands;
using DAYA.Cloud.Framework.V2.Application.Outbox;
using DAYA.Cloud.Framework.V2.AzureServiceBus;
using DAYA.Cloud.Framework.V2.Cosmos;
using DAYA.Cloud.Framework.V2.Cosmos.Abstractions;
using DAYA.Cloud.Framework.V2.DirectOperations;
using DAYA.Cloud.Framework.V2.DirectOperations.Behaviors;
using DAYA.Cloud.Framework.V2.DirectOperations.Contracts;
using DAYA.Cloud.Framework.V2.DirectOperations.Repositories;
using DAYA.Cloud.Framework.V2.Infrastructure.AzureServiceBus;
using DAYA.Cloud.Framework.V2.Infrastructure.Configuration.Processing.InternalCommands;
using DAYA.Cloud.Framework.V2.Infrastructure.Configuration.Processing.Outbox;
using DAYA.Cloud.Framework.V2.Infrastructure.EventBus;
using DAYA.Cloud.Framework.V2.Infrastructure.Processing.CommandPipelines;
using DAYA.Cloud.Framework.V2.Infrastructure.Processing.QueryPipelines;
using DAYA.Cloud.Framework.V2.ServiceBus;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DAYA.Cloud.Framework.V2.Infrastructure.Configuration;

public static class ServiceCollectionExtensions
{    /// <summary>
     /// Registers MediatR services with the .NET Core dependency injection container </summary>
    public static IServiceCollection AddDayaMediator(this IServiceCollection services, IConfiguration configuration, Assembly infrastructureAssembly, Assembly applicationAssembly)
    {
        Assembly[] assemblies = new[]
        {
            infrastructureAssembly,
            applicationAssembly
        };

        RegisterValidations(services, assemblies);
        services.AddSingleton<IServiceModule, ServiceModule>();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterGenericHandlers = true;
            cfg.RegisterServicesFromAssemblies(assemblies);
            cfg.AddOpenBehavior(typeof(DirectCommandLoggingBehavior<,>), ServiceLifetime.Transient);
            cfg.AddOpenBehavior(typeof(DirectCommandTransactionBehavior<,>), ServiceLifetime.Transient);
            cfg.AddOpenBehavior(typeof(DirectCommandDomainEventBehavior<,>), ServiceLifetime.Transient);
            cfg.AddOpenBehavior(typeof(QueryValidationBehavior<,>), ServiceLifetime.Transient);
            cfg.AddOpenBehavior(typeof(CommandValidationBehavior<,>), ServiceLifetime.Transient);
            //cfg.AddOpenBehavior(typeof(NotificationUnitOfWorkBehavior<,>), ServiceLifetime.Transient);
        });

        services.AddScoped<ICosmosEntityChangeTracker, CosmosEntityChangeTracker>();
        services.AddScoped(typeof(ICosmosRepository<,>), typeof(CosmosRepository<,>));

        services.AddScoped<ICommandsScheduler, CommandsScheduler>();
        services.AddSingleton<IContainerFactory, ContainerFactory>();
        services.AddScoped<IDirectUnitOfWork, DirectUnitOfWork>();

        var serviceName = configuration.GetValue<string>("ServiceName") ?? "DayaService";
        var outboxQueueName = $"{serviceName}-outboxmessages".ToLower();
        var internalCommandMessageQueue = $"{serviceName}-internalcommandmessage".ToLower();

        services.AddSingleton(new OutboxConfig { Name = outboxQueueName });
        services.AddSingleton(new InternalCommandConfig { QueueName = internalCommandMessageQueue });

        services.AddSingleton<IQueueClientFactory, QueueClientFactory>();
        services.AddSingleton<ITopicClientFactory, TopicClientFactory>();
        services.AddSingleton<IQueueMessagePublisher, QueueMessagePublisher>();
        services.AddSingleton(new ServiceBusTopicPublisherCompressionOptions
        {
            EnableCompression = false
        });

        services.AddSingleton(new ServiceBusQueuePublisherCompressionOptions
        {
            EnableCompression = false
        });

        services.AddSingleton<IOutboxMessageRepository, OutboxMessageRepository>();
        services.AddSingleton<IInternalMessageRepository, InternalMessageRepository>();
        services.AddSingleton<IOutboxMessageProcessor, OutboxMessageProcessor>();
        services.AddSingleton<IInternalMessageProcessor, InternalMessageProcessor>();

        services.AddSingleton<IEventBus, AzureServiceBusEventBus>();

        services.AddSingleton<IApplicationAssemblyResolver>(new ApplicationAssemblyResolver(applicationAssembly));

        return services;
    }

    public static IServiceCollection AddDayaHostedServices(this IServiceCollection services)
    {
        services.AddHostedService<OutboxMessageBackgroundService>();
        services.AddHostedService<InternalCommandMessageBackgroundService>();
        return services;
    }

    private static void RegisterValidations(IServiceCollection services, params Assembly[] assemblies)
    {
        // Register validators from assemblies manually
        foreach (var assembly in assemblies)
        {
            // Find all validator types in the assembly
            var validatorTypes = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract)
                .Where(t => t.GetInterfaces()
                    .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidator<>)))
                .ToList();

            // Register each validator with its interface
            foreach (var validatorType in validatorTypes)
            {
                var validatorInterface = validatorType.GetInterfaces()
                    .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidator<>));
                services.AddTransient(validatorInterface, validatorType);
            }
        }
    }
}