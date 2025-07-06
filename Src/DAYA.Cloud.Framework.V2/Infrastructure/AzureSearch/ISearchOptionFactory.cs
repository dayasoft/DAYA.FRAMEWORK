using Azure.Search.Documents;
using DAYA.Cloud.Framework.V2.Application.Contracts;

namespace DAYA.Cloud.Framework.V2.Infrastructure.AzureSearch;

public interface ISearchOptionFactory<TRequest, out TResult>
    where TRequest : ISearchQuery<TResult>
{
    SearchOptions Create(TRequest request);
}
