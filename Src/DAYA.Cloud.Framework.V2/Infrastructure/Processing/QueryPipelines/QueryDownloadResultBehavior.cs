using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DAYA.Cloud.Framework.V2.Application.Contracts;
using DAYA.Cloud.Framework.V2.Infrastructure.AzureSearch;
using DAYA.Cloud.Framework.V2.Infrastructure.Configuration;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DAYA.Cloud.Framework.V2.Infrastructure.Processing.QueryPipelines
{
    internal class QueryDownloadResultBehavior<TRequest, TResult> : IPipelineBehavior<TRequest, TResult>
    where TRequest : DownloadableQuery<TResult>
    where TResult : DownloadSearchDto
    {
        private readonly ILogger _logger;

        private readonly ISearchOptionColumns<TRequest, TResult> _searchOptionColumns;

        public QueryDownloadResultBehavior(
            ILogger logger,
            IEnumerable<ISearchOptionColumns<TRequest, TResult>> searchOptionColumns
            )
        {
            _logger = logger;
            _searchOptionColumns = searchOptionColumns.SingleOrDefault();
            if (_searchOptionColumns is null)
            {
                throw new Exception($"Single Query columns factory not found for {nameof(TRequest)}");
            }
        }

        public async Task<TResult> Handle(TRequest request, RequestHandlerDelegate<TResult> next, CancellationToken cancellationToken)
        {
            await next();

            _logger.LogInformation("Calling download factory...");
            var query = request.SearchQuery;
            using var scope = ServiceCompositionRoot.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var result = await mediator.Send(query, cancellationToken);

            var items = result.GetType().GetProperty("SearchResult").GetValue(result);

            var forFields = _searchOptionColumns.GetFields();

            var csvMaker = new CsvMaker();
            var output = csvMaker.CreateCsv(forFields, items);

            var download = new DownloadSearchDto
            {
                Output = output,
                FileName = csvMaker.CreateFileName(_searchOptionColumns.GetFileName()),
                FileContentType = _searchOptionColumns.GetFileType()
            };

            return download as TResult;
        }
    }
}