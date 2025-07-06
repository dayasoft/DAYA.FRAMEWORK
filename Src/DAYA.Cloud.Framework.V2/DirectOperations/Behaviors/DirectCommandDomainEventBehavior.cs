using System;
using System.Threading;
using System.Threading.Tasks;
using DAYA.Cloud.Framework.V2.DirectOperations.Contracts;
using MediatR;

namespace DAYA.Cloud.Framework.V2.DirectOperations.Behaviors
{
    internal class DirectCommandDomainEventBehavior<T, TResult> : IPipelineBehavior<T, TResult>
        where T : IDirectCommand<TResult>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IDirectUnitOfWork _unitOfWork;

        public DirectCommandDomainEventBehavior(IServiceProvider serviceProvider, IDirectUnitOfWork unitOfWork)
        {
            _serviceProvider = serviceProvider;
            _unitOfWork = unitOfWork;
        }

        public async Task<TResult> Handle(T request, RequestHandlerDelegate<TResult> next, CancellationToken cancellationToken)
        {
            var result = await next();

            await _unitOfWork.CommitAsync(cancellationToken);

            return result;
        }
    }
}