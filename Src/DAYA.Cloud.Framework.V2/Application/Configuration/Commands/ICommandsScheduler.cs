using System.Threading.Tasks;
using DAYA.Cloud.Framework.V2.Application.Contracts;
using DAYA.Cloud.Framework.V2.DirectOperations.Contracts;

namespace DAYA.Cloud.Framework.V2.Application.Configuration.Commands;

public interface ICommandsScheduler
{
    Task EnqueueAsync(IDirectCommand command);

    Task EnqueueAsync<TResult>(IDirectCommand<TResult> command);

    Task EnqueueAsync<TResult>(IUpdateSearchCommand<TResult> command);

    Task EnqueueAsync<TResult>(IRemoveSearchCommand<TResult> command);

    Task EnqueueAsync(IDirectCommand command, string? sessionId);

    Task EnqueueAsync<TResult>(IDirectCommand<TResult> command, string? sessionId);

    Task EnqueueAsync<TResult>(IUpdateSearchCommand<TResult> command, string? sessionId);

    Task EnqueueAsync<TResult>(IRemoveSearchCommand<TResult> command, string? sessionId);
}