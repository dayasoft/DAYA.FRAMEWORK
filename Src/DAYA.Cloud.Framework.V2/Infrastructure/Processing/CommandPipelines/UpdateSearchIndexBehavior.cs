using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;
using DAYA.Cloud.Framework.V2.Application.Contracts;
using DAYA.Cloud.Framework.V2.Infrastructure.AzureSearch;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DAYA.Cloud.Framework.V2.Infrastructure.Processing.CommandPipelines;

internal class UpdateSearchIndexBehavior<TRequest, TResult> : IPipelineBehavior<TRequest, TResult> where TRequest : IUpdateSearchCommand<TResult>
{
    private readonly ISearchIndexClientFactory _searchIndexClientFactory;
    private readonly ILogger _logger;
    private readonly ISearchIndexCreator<TRequest, TResult> _searchIndexCreator;

    public UpdateSearchIndexBehavior(
        ISearchIndexClientFactory searchIndexClientFactory,
        ILogger logger,
        IEnumerable<ISearchIndexCreator<TRequest, TResult>> searchIndexCreators)
    {
        _searchIndexClientFactory = searchIndexClientFactory;
        _logger = logger;
        _searchIndexCreator = searchIndexCreators.SingleOrDefault();
        if (_searchIndexCreator is null)
        {
            throw new Exception($"Index creator not found for {nameof(TRequest)}");
        }
    }

    public async Task<TResult> Handle(TRequest request, RequestHandlerDelegate<TResult> next, CancellationToken cancellationToken)
    {
        var searchModel = await next();

        var action = new IndexDocumentsAction<TResult>(IndexActionType.MergeOrUpload, searchModel);

        var batch = IndexDocumentsBatch.Create(action);

        var searchIndexClient = _searchIndexClientFactory.Create();

        await CreateIndexAsync(searchIndexClient, request);

        var searchClient = searchIndexClient.GetSearchClient(request.IndexName);
        await searchClient.IndexDocumentsAsync(
            batch,
            new IndexDocumentsOptions()
            {
                ThrowOnAnyError = false,
            },
            cancellationToken);

        return searchModel;
    }

    public async Task CreateIndexAsync(
        SearchIndexClient searchIndexClient,
        IUpdateSearchCommand<TResult> updateSearchCommand)
    {
        if (searchIndexClient.GetIndexes().Any(x => x.Name == updateSearchCommand.IndexName))
        {
            return;
        }

        var searchFields = _searchIndexCreator.GetSearchFields();

        await searchIndexClient.CreateIndexAsync(new SearchIndex(updateSearchCommand.IndexName, searchFields));
    }
}
