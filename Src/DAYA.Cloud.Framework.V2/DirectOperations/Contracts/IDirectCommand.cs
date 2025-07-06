using MediatR;
using System;

namespace DAYA.Cloud.Framework.V2.DirectOperations.Contracts
{
    public interface IDirectCommand<out TResult> : IRequest, IRequest<TResult>
    {
        Guid InternalProcessId { get; }
    }

    public interface IDirectCommand : IDirectCommand<Unit>
    {
    }
}