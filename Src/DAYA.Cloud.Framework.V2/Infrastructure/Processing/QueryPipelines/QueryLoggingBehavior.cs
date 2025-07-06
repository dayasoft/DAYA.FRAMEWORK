using System;
using System.Threading;
using System.Threading.Tasks;
using DAYA.Cloud.Framework.V2.Application.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DAYA.Cloud.Framework.V2.Infrastructure.Processing.QueryPipelines;

internal class QueryLoggingBehavior<T, TResult> : IPipelineBehavior<T, TResult> where T : IQuery<TResult>
{
    private readonly ILogger _logger;

    public QueryLoggingBehavior(ILogger logger)
    {
        _logger = logger;
    }

    public async Task<TResult> Handle(T request, RequestHandlerDelegate<TResult> next, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"{request.GetType().Name} is processing: {Environment.NewLine}{JsonConvert.SerializeObject(request, Formatting.Indented)}");
        try
        {
            TResult result = await next();
            if (typeof(TResult) != typeof(Unit))
            {
                _logger.LogInformation($"Result: {Environment.NewLine}{JsonConvert.SerializeObject(result, Formatting.Indented)}");
            }
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Unhandled Exception:{Environment.NewLine}{ex.ToString()}");
            throw;
        }
        finally
        {
            _logger.LogDebug($"{request.GetType().Name} is processed.");
        }
    }
}