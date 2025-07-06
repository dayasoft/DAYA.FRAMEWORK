using MediatR;

namespace DAYA.Cloud.Framework.V2.DirectOperations.Contracts;

public interface IDirectCommandHandler<in TCommand> : IRequestHandler<TCommand> where TCommand : IDirectCommand
{
}

public interface IDirectCommandHandler<in TCommand, TResult> :
    IRequestHandler<TCommand, TResult> where TCommand : IDirectCommand<TResult>
{
}