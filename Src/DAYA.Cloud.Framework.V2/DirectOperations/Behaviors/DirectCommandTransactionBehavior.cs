using System;
using System.Threading;
using System.Threading.Tasks;
using DAYA.Cloud.Framework.V2.DirectOperations.Contracts;
using MediatR;

namespace DAYA.Cloud.Framework.V2.DirectOperations.Behaviors;

internal class DirectCommandTransactionBehavior<T, TResult> : IPipelineBehavior<T, TResult>
    where T : IDirectCommand<TResult>
{
    private readonly ICosmosEntityChangeTracker _cosmosEntityChangeTracker;
    private readonly IServiceProvider _serviceProvider;

    public DirectCommandTransactionBehavior(IServiceProvider serviceProvider, ICosmosEntityChangeTracker cosmosEntityChangeTracker)
    {
        _serviceProvider = serviceProvider;
        _cosmosEntityChangeTracker = cosmosEntityChangeTracker;
    }

    public async Task<TResult> Handle(T request, RequestHandlerDelegate<TResult> next, CancellationToken cancellationToken)
    {
        var result = await next(cancellationToken);

        var changedEntities = _cosmosEntityChangeTracker.GetTrackedEntities();

        return result;
    }
}