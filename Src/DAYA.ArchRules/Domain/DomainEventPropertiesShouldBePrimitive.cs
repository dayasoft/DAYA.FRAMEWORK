using DAYA.ArchRules.Utilities;

namespace DAYA.ArchRules.Domain
{
    class DomainEventPropertiesShouldBePrimitive : ArchRule
    {
        internal override void Check()
        {
            var definedTypes = DomainDefinedTypes;
            var result = DomainEvents
                .Should()
                .HaveOnlySimpleOrDefinedProperties(definedTypes);

            AssertArchTestResult(result);
        }
    }
}
