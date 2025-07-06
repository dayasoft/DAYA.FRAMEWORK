using DAYA.Cloud.Framework.V2.DirectOperations.Contracts;
using System.Threading.Tasks;

namespace DAYA.Cloud.Framework.V2.Application.Contracts;

public interface IServiceModule
{
    Task<TResult> ExecuteQueryAsync<TResult>(IQuery<TResult> query);

    Task ExecuteCommandAsync(IDirectCommand command);

    Task<TResult> ExecuteCommandAsync<TResult>(IDirectCommand<TResult> command);
}