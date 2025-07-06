using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.Search.Documents.Models;
using DAYA.Cloud.Framework.V2.Application.Contracts;
using DAYA.Cloud.Framework.V2.Infrastructure.AzureSearch;
using DAYA.Cloud.Framework.V2.Infrastructure.Configuration;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DAYA.Cloud.Framework.V2.Infrastructure.Processing.QueryPipelines;

internal class QuerySearchResultBehavior<TRequest, TResult> : IPipelineBehavior<TRequest, TResult>
    where TRequest : SearchQuery<TResult>
{
    private readonly ISearchIndexClientFactory _searchIndexClientFactory;
    private readonly ILogger _logger;
    private readonly ISearchOptionFactory<TRequest, TResult> _searchOptionFactory;
    private readonly IQueryFactory<TRequest> _queryFactory;
    private readonly AzureSearchContext _azureSearchContext;

    public QuerySearchResultBehavior(
        ISearchIndexClientFactory searchIndexClientFactory,
        ILogger logger,
        IEnumerable<ISearchOptionFactory<TRequest, TResult>> searchOptionFactories,
        IEnumerable<IQueryFactory<TRequest>> queryFactories,
        AzureSearchContext azureSearchContext)
    {
        _searchIndexClientFactory = searchIndexClientFactory;
        _logger = logger;
        _searchOptionFactory = searchOptionFactories.SingleOrDefault();
        if (_searchOptionFactory is null)
        {
            throw new Exception($"Single Search option factory not found for {nameof(TRequest)}");
        }
        _queryFactory = queryFactories.SingleOrDefault();
        if (_queryFactory is null)
        {
            throw new Exception($"Single Query factory not found for {nameof(TRequest)}");
        }
        _azureSearchContext = azureSearchContext;
    }

    public async Task<TResult> Handle(TRequest request, RequestHandlerDelegate<TResult> next, CancellationToken cancellationToken)
    {
        var searchOptions = _searchOptionFactory.Create(request);

        var searchIndexClient = _searchIndexClientFactory.Create();

        if (searchIndexClient.GetIndexes().All(x => x.Name != request.IndexName))
        {
            _logger.LogWarning($"Index not found in azure search. Index name: {request.IndexName}");
            return default;
        }

        var keyword = request.Keyword.Trim();

        if (string.IsNullOrEmpty(keyword) == false)
        {
            if (request.UseFuzzySearch)
            {
                var searchKeywords = keyword.Split(' ');
                var fuzzyKeywords = searchKeywords.Select(x => $"{x}~1");
                keyword = $"{string.Join(" ", fuzzyKeywords)}";
            }
        }

        _azureSearchContext.SearchResults = await searchIndexClient
                .GetSearchClient(request.IndexName)
                .SearchAsync<SearchDocument>(keyword, searchOptions, cancellationToken);

        _logger.LogInformation("Calling query factory...");
        var query = _queryFactory.Create(request);
        if (query is not null)
        {
            using var scope = ServiceCompositionRoot.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var result = await mediator.Send(query, cancellationToken);
            _azureSearchContext.QueryResult = result;
        }

        return await next();
    }
}