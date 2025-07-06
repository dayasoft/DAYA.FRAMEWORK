using Azure.Search.Documents.Indexes;

namespace DAYA.Cloud.Framework.V2.Infrastructure.AzureSearch;

public interface ISearchIndexClientFactory
{
    SearchIndexClient Create();
}
