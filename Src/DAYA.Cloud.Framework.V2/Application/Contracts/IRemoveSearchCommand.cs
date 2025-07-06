namespace DAYA.Cloud.Framework.V2.Application.Contracts;

public interface IRemoveSearchCommand<out TResult> : ICommand<TResult>
{
    string IndexName { get; }
}