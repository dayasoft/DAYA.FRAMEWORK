// filepath: c:\Projects\DayaSolutions\DAYA.Framework.V2\Src\DAYA.Cloud.Framework.V2\Infrastructure\Mediation\MediationExtensions.cs
using System.Reflection;
using DAYA.Cloud.Framework.V2.Infrastructure.Processing.CommandPipelines;
using DAYA.Cloud.Framework.V2.Infrastructure.Processing.QueryPipelines;
using Microsoft.Extensions.DependencyInjection;

namespace DAYA.Cloud.Framework.V2.Infrastructure.Mediation
{
    public static class MediationExtensions
    {
        /// <summary>
        /// Adds MediatR services and registers handlers from the specified assemblies
        /// </summary>
        public static IServiceCollection AddDayaMediation(this IServiceCollection services, params Assembly[] assemblies)
        {
            // Domain events
            //services.AddScoped<IDomainEventsDispatcher, DomainEventsDispatcher>();
            //services.AddScoped<ICommandsScheduler, CommandsScheduler>();
            //services.AddScoped<IDomainEventsAccessor, DomainEventsAccessor>();
            //services.AddScoped<ICosmosEntityChangeTracker, CosmosEntityChangeTracker>();
            //services.AddScoped(typeof(ICosmosRepository<,>), typeof(CosmosRepository<,>));
            //services.AddScoped<IDirectUnitOfWork, DirectUnitOfWork>();

            //services.AddSingleton<OutboxConfig>();
            //services.AddSingleton<InternalCommandConfig>();

            // Register MediatR
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(assemblies);
                //cfg.AddOpenBehavior(typeof(RequestPostProcessorBehavior<,>));
                //cfg.AddOpenBehavior(typeof(RequestPreProcessorBehavior<,>));
                //cfg.AddOpenBehavior(typeof(DirectCommandLoggingBehavior<,>));
                //cfg.AddOpenBehavior(typeof(DirectCommandTransactionBehavior<,>));
                //cfg.AddOpenBehavior(typeof(DirectCommandDomainEventBehavior<,>));
                //cfg.AddOpenBehavior(typeof(NotificationLoggingBehavior<,>));
                //cfg.AddOpenBehavior(typeof(QueryValidationBehavior<,>));
                cfg.AddOpenBehavior(typeof(QueryLoggingBehavior<,>));
                cfg.AddOpenBehavior(typeof(QueryCachingBehavior<,>));
                cfg.AddOpenBehavior(typeof(QuerySearchResultBehavior<,>));
                cfg.AddOpenBehavior(typeof(QueryDownloadResultBehavior<,>));
                cfg.AddOpenBehavior(typeof(UpdateSearchIndexBehavior<,>));
                cfg.AddOpenBehavior(typeof(RemoveFromSearchIndexBehavior<,>));
                cfg.AddOpenBehavior(typeof(CommandValidationBehavior<,>));
                cfg.AddOpenBehavior(typeof(CommandUnitOfWorkBehavior<,>));
                //cfg.AddOpenBehavior(typeof(NotificationUnitOfWorkBehavior<,>));
            });

            //services.Decorate(typeof(INotificationHandler<>), typeof(DomainEventsDispatcherNotificationHandlerDecorator<>));

            return services;
        }
    }
}