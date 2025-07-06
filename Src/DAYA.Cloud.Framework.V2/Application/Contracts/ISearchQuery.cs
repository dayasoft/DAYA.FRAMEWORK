namespace DAYA.Cloud.Framework.V2.Application.Contracts;

public interface ISearchQuery<out TResult> : IQuery<TResult>
{
    string IndexName { get; }
    string Keyword { get; }
}