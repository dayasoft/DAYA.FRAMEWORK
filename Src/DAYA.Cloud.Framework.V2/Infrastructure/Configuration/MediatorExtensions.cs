using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DAYA.Cloud.Framework.V2.Infrastructure.Configuration
{
    public static class MediatorExtensions
    {
        public static void SendAndForget<T>(this IMediator mediator, T request, ILogger logger = null)
            where T : IRequest
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await mediator.Send(request);
                }
                catch (Exception ex)
                {
                    logger?.LogError(ex, "Fire-and-forget command failed: {CommandType}", typeof(T).Name);
                }
            });
        }
    }
}