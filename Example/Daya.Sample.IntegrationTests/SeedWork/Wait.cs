using System.Runtime.ExceptionServices;
using Polly;
using Polly.Retry;

namespace Daya.Sample.IntegrationTests.SeedWork
{
    internal class Wait
    {
        private static readonly AsyncRetryPolicy Policy = Polly.Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(new[]
            {
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(2),
                TimeSpan.FromSeconds(3),
            });

        public static async Task Until(
            Func<Task> func)
        {
            var result = await Policy.ExecuteAndCaptureAsync(async () =>
            {
                await func.Invoke().ConfigureAwait(false);
            }).ConfigureAwait(false);

            if (result.Outcome == OutcomeType.Failure)
            {
                ExceptionDispatchInfo.Capture(result.FinalException).Throw();
            }
        }
    }
}