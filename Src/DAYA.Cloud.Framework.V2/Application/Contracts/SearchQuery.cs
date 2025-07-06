namespace DAYA.Cloud.Framework.V2.Application.Contracts;

public abstract record SearchQuery<TResult> : SearchQuery, ISearchQuery<TResult>
{
    public string IndexName { get; }
    public string Keyword { get; }
    public bool UseFuzzySearch { get; }

    protected SearchQuery(string indexName, string keyword, bool useFuzzySearch = false) : base()
    {
        IndexName = indexName;
        Keyword = keyword;
        UseFuzzySearch = useFuzzySearch;
    }
}