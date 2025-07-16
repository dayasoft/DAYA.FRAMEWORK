using DAYA.ArchRules.Utilities;

namespace DAYA.ArchRules.Domain
{
    class DomainEventsShouldBeImmutable : ArchRule
    {
        internal override void Check()
        {
            var result =
                DomainEvents
                .Should()
                .BeInitOnly();

            AssertArchTestResult(result);
        }
    }
}
