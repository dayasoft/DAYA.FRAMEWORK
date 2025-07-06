using Azure.Search.Documents.Models;

namespace DAYA.Cloud.Framework.V2.Infrastructure.AzureSearch;

public class AzureSearchContext
{
    public SearchResults<SearchDocument> SearchResults { get; internal set; }
    public object QueryResult { get; internal set; }
}
