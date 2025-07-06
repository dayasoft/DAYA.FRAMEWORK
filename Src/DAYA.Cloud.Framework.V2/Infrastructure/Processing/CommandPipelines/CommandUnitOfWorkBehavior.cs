using System.Threading;
using System.Threading.Tasks;
using DAYA.Cloud.Framework.V2.Application.Contracts;
using DAYA.Cloud.Framework.V2.Domain;
using MediatR;

namespace DAYA.Cloud.Framework.V2.Infrastructure.Processing.CommandPipelines;

internal class CommandUnitOfWorkBehavior<T, TResult> : IPipelineBehavior<T, TResult> where T : ICommand<TResult>
{
    private readonly IUnitOfWork _unitOfWork;

    public CommandUnitOfWorkBehavior(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<TResult> Handle(T request, RequestHandlerDelegate<TResult> next, CancellationToken cancellationToken)
    {
        var result = await next();
        await _unitOfWork.CommitAsync(cancellationToken);

        return result;
    }
}