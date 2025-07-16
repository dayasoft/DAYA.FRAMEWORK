using DAYA.ArchRules.Utilities;

namespace DAYA.ArchRules.IntegrationEvents
{
    class IntegrationEventPropertiesShouldBePrimitive : ArchRule
    {
        internal override void Check()
        {
            var result = IntegrationEvents
                .Should()
                .HaveOnlySimpleOrDefinedProperties(DomainDefinedTypes);

            AssertArchTestResult(result);
        }
    }
}
