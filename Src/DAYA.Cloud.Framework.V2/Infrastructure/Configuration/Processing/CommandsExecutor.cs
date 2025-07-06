using System.Threading.Tasks;
using DAYA.Cloud.Framework.V2.DirectOperations.Contracts;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace DAYA.Cloud.Framework.V2.Infrastructure.Configuration.Processing;

public static class CommandsExecutor
{
    public static async Task Execute(IDirectCommand command)
    {
        using var scope = ServiceCompositionRoot.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        await mediator.Send(command);
    }

    public static async Task<TResult> Execute<TResult>(IDirectCommand<TResult> command)
    {
        using var scope = ServiceCompositionRoot.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        return await mediator.Send<TResult>(command);
    }

    public static async Task ExecuteAndForget(IDirectCommand command)
    {
        using var scope = ServiceCompositionRoot.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        mediator.SendAndForget(command);
        await Task.CompletedTask;
    }
}