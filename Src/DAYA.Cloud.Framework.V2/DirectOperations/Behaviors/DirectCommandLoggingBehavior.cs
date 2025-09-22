using System;
using System.Threading;
using System.Threading.Tasks;
using DAYA.Cloud.Framework.V2.Application.Exceptions;
using DAYA.Cloud.Framework.V2.DirectOperations.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DAYA.Cloud.Framework.V2.DirectOperations.Behaviors;

internal class DirectCommandLoggingBehavior<T, TResult> : IPipelineBehavior<T, TResult>
    where T : IDirectCommand<TResult>
{
    private readonly ILogger<DirectCommandLoggingBehavior<T, TResult>> _logger;

    public DirectCommandLoggingBehavior(ILogger<DirectCommandLoggingBehavior<T, TResult>> logger)
    {
        _logger = logger;
    }

    public async Task<TResult> Handle(T request, RequestHandlerDelegate<TResult> next, CancellationToken cancellationToken)
    {
        string processId = request is IDirectCommand command ? command.InternalProcessId.ToString() : "N/A";

        _logger.LogInformation("{requestName}[{processId}] is processing: {environment}{request}",
            typeof(T).Name,
            processId,
            Environment.NewLine,
            JsonConvert.SerializeObject(request, Formatting.Indented));
        try
        {
            TResult result = await next(cancellationToken);
            if (typeof(TResult) != typeof(Unit))
            {
                _logger.LogInformation("Result: {environment}{result}",
                    Environment.NewLine,
                    result);
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
            _logger.LogInformation($"request {typeof(T).Name} is processed.");
        }
    }
}