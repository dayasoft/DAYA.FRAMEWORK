using DAYA.ArchRules.Utilities;

namespace DAYA.ArchRules.IntegrationEvents
{
    class IntegrationEventsCanNotHaveIdProperty : ArchRule
    {
        internal override void Check()
        {
            var result = IntegrationEvents
                .ShouldNot()
                .HavePropertyWithName("id");

            AssertArchTestResult(result);
        }
    }
}
