using System;

namespace DAYA.Cloud.Framework.V2.Application.Contracts;

public abstract record Command : ICommand
{
    public Guid InternalProcessId { get; }

    protected Command()
    {
        this.InternalProcessId = Guid.NewGuid();
    }
}

public abstract record Command<TResult> : ICommand<TResult>
{
    public Guid InternalProcessId { get; }

    protected Command()
    {
        this.InternalProcessId = Guid.NewGuid();
    }
}