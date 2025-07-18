using Microsoft.Extensions.DependencyInjection;

namespace Daya.Sample.IntegrationTests.Configuration
{
    internal static class DomainServiceCollectionExtension
    {
        internal static IServiceCollection RegisterDomainServices(this IServiceCollection services)
        {
            // register your domain services here

            return services;
        }
    }
}