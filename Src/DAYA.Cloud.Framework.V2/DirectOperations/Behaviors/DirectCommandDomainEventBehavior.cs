using System;
using System.Threading;
using System.Threading.Tasks;
using DAYA.Cloud.Framework.V2.DirectOperations.Contracts;
using DAYA.Cloud.Framework.V2.DirectOperations.Exceptions;
using MediatR;

namespace DAYA.Cloud.Framework.V2.DirectOperations.Behaviors
{
    internal class DirectCommandDomainEventBehavior<T, TResult> : IPipelineBehavior<T, TResult>
        where T : IDirectCommand<TResult>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IDirectUnitOfWork _unitOfWork;
        private const int MaxRetries = 5;
        private const int BaseDelayMs = 100;

        public DirectCommandDomainEventBehavior(IServiceProvider serviceProvider, IDirectUnitOfWork unitOfWork)
        {
            _serviceProvider = serviceProvider;
            _unitOfWork = unitOfWork;
        }

        public async Task<TResult> Handle(T request, RequestHandlerDelegate<TResult> next, CancellationToken cancellationToken)
        {
            var retryCount = 0;

            while (true)
            {
                try
                {
                    var result = await next(cancellationToken);
                    await _unitOfWork.CommitAsync(cancellationToken);
                    return result;
                }
                catch (ConcurrencyException) when (retryCount < MaxRetries)
                {
                    retryCount++;
                    var delay = CalculateDelay(retryCount);

                    await Task.Delay(delay, cancellationToken);
                }
                catch (ConcurrencyException ex) when (retryCount >= MaxRetries)
                {
                    throw new InvalidOperationException(
                        $"Command failed after {MaxRetries} retry attempts due to concurrency conflicts. " +
                        $"This indicates high contention on the resource.", ex);
                }
            }
        }

        private static int CalculateDelay(int retryCount)
        {
            // Exponential backoff with jitter
            var exponentialDelay = BaseDelayMs * Math.Pow(2, retryCount - 1);
            var jitter = new Random().Next(0, (int)(exponentialDelay * 0.1));
            return (int)(exponentialDelay + jitter);
        }
    }
}