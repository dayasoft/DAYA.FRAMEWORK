using System;

namespace DAYA.Cloud.Framework.V2.Application.Contracts;

public abstract record RemoveSearchCommand<TResult> : IRemoveSearchCommand<TResult>
{
    public Guid InternalProcessId { get; }
    public string IndexName { get; }

    protected RemoveSearchCommand(string indexName)
    {
        IndexName = indexName;
        this.InternalProcessId = Guid.NewGuid();
    }
}