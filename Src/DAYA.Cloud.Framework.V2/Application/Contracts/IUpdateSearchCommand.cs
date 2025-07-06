namespace DAYA.Cloud.Framework.V2.Application.Contracts;

public interface IUpdateSearchCommand<out TResult> : ICommand<TResult>
{
    string IndexName { get; }
}