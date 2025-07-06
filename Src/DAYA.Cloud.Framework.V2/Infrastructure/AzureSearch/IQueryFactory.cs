using DAYA.Cloud.Framework.V2.Application.Contracts;

namespace DAYA.Cloud.Framework.V2.Infrastructure.AzureSearch;

public interface IQueryFactory<TRequest>
    where TRequest : SearchQuery
{
    Query Create(TRequest request);
}
