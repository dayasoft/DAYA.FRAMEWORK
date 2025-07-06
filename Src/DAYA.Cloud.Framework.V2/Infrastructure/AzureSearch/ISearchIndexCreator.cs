using System.Collections.Generic;
using Azure.Search.Documents.Indexes.Models;
using DAYA.Cloud.Framework.V2.Application.Contracts;

namespace DAYA.Cloud.Framework.V2.Infrastructure.AzureSearch;

public interface ISearchIndexCreator<TRequest, out TResult>
    where TRequest : IUpdateSearchCommand<TResult>
{
    IEnumerable<SearchField> GetSearchFields();
}
