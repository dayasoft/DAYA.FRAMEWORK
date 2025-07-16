using DAYA.ArchRules.Utilities;

namespace DAYA.ArchRules.IntegrationEvents
{
    class IntegrationEventMustHaveAPublicConstructor : ArchRule
    {
        internal override void Check()
        {
            var result = IntegrationEvents
                .Should()
                .HaveAPublicConstructor();

            AssertArchTestResult(result);
        }
    }
}
