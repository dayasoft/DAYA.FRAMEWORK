using DAYA.ArchRules.Utilities;

namespace DAYA.ArchRules.IntegrationEvents
{
    class IntegrationEventPropertiesShouldBePublic : ArchRule
    {
        internal override void Check()
        {
            var result = IntegrationEvents
                .Should()
                .HaveOnlyPublicProperties();

            AssertArchTestResult(result);
        }
    }
}
