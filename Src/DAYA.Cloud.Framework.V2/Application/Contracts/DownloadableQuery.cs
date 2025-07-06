namespace DAYA.Cloud.Framework.V2.Application.Contracts;

public abstract record DownloadableQuery<TResult> : DownloadableQuery, IQuery<TResult>
    where TResult : DownloadSearchDto
{
    public SearchQuery SearchQuery { get; }

    protected DownloadableQuery(SearchQuery searchQuery)
        : base()
    {
        SearchQuery = searchQuery;
    }
}