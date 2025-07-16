using DAYA.ArchRules.Utilities;

namespace DAYA.ArchRules.IntegrationEvents
{
    class IntegrationEventsCanNotBeUsedAsBaseClass : ArchRule
    {
        internal override void Check()
        {
            var result = IntegrationEvents
                .ShouldNot()
                .UsedAsBaseClass(AllTypes);

            AssertArchTestResult(result);
        }
    }
}
