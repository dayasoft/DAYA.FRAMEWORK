using System;
using DAYA.Cloud.Framework.V2.DirectOperations.Contracts;

namespace DAYA.Cloud.Framework.V2.DirectOperations;

public abstract record DirectCommand : IDirectCommand
{
    public Guid InternalProcessId { get; }

    protected DirectCommand()
    {
        this.InternalProcessId = Guid.NewGuid();
    }
}

public abstract record DirectCommand<TResult> : IDirectCommand<TResult>
{
    public Guid InternalProcessId { get; }

    protected DirectCommand()
    {
        this.InternalProcessId = Guid.NewGuid();
    }
}