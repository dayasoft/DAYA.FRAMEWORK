using System;
using System.Threading;
using System.Threading.Tasks;
using DAYA.Cloud.Framework.V2.Application.Contracts;
using DAYA.Cloud.Framework.V2.Application.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DAYA.Cloud.Framework.V2.Infrastructure.Processing.CommandPipelines;

internal class CommandLoggingBehavior<T, TResult> : IPipelineBehavior<T, TResult> where T : ICommand<TResult>
{
    private readonly ILogger _logger;

    public CommandLoggingBehavior(ILogger logger)
    {
        _logger = logger;
    }

    public async Task<TResult> Handle(T request, RequestHandlerDelegate<TResult> next, CancellationToken cancellationToken)
    {
        string requestName = request.GetType().Name;

        _logger.LogInformation("{requestName} is processing: {environment}{request}",
            requestName,
            Environment.NewLine,
            JsonConvert.SerializeObject(request, Formatting.Indented)
        );

        try
        {
            TResult result = await next();

            if (typeof(TResult) != typeof(Unit))
            {
                _logger.LogInformation("Result: {environment}{result}", Environment.NewLine, result);
            }

            return result;
        }
        catch (EntityAlreadyExistsException ex)
        {
            _logger.LogError(ex.ToString());
            throw;
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogError(ex.ToString());
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            throw;
        }
        finally
        {
            _logger.LogInformation($"request {requestName} is processed.");
        }
    }
}