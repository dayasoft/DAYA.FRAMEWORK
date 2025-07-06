using System.Threading.Tasks;
using DAYA.Cloud.Framework.V2.Application.Contracts;
using DAYA.Cloud.Framework.V2.DirectOperations.Contracts;
using DAYA.Cloud.Framework.V2.Infrastructure.Configuration;
using DAYA.Cloud.Framework.V2.Infrastructure.Configuration.Processing;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace DAYA.Cloud.Framework.V2.Infrastructure;

public class ServiceModule : IServiceModule
{
    public async Task<TResult> ExecuteQueryAsync<TResult>(IQuery<TResult> query)
    {
        using var scope = ServiceCompositionRoot.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        return await mediator.Send(query);
    }

    public async Task<TResult> ExecuteCommandAsync<TResult>(IDirectCommand<TResult> command)
    {
        return await CommandsExecutor.Execute(command);
    }

    public async Task ExecuteCommandAsync(IDirectCommand command)
    {
        await CommandsExecutor.Execute(command);
    }
}