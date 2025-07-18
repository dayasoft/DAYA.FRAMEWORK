using Daya.Sample.Infrastructure.Configuration.CosmosDatabase;
using Microsoft.Extensions.Hosting;

namespace Jobbiplace.SubscriptionService.DataReset
{
    public class SharedTaskCoordinator : IHostedService
    {
        private readonly IHostApplicationLifetime _lifetime;
        private readonly IDatabaseConfigurrator _dataBaseReset;

        public SharedTaskCoordinator(IHostApplicationLifetime lifetime, IDatabaseConfigurrator dataBaseReset)
        {
            _lifetime = lifetime;
            _dataBaseReset = dataBaseReset;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("STARTING DATA RESET");

            var tasks = new[]
            {
                Task.Run(() => _dataBaseReset.ExecuteAsync(cancellationToken), cancellationToken),
            };

            await Task.WhenAll(tasks);

            _lifetime.StopApplication();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("FINISHING DATA RESET");
            return Task.CompletedTask;
        }
    }
}