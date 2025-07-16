using DAYA.ArchRules.Utilities;

namespace DAYA.ArchRules.IntegrationEvents
{
    class IntegrationEventShouldBeImmutable : ArchRule
    {
        internal override void Check()
        {
            var result = IntegrationEvents
                .Should()
                .BeInitOnly();

            AssertArchTestResult(result);
        }
    }
}
