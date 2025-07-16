using System.Diagnostics.CodeAnalysis;

namespace DAYA.ArchRules.Application.Notifications
{
    class NotificationHandlers_can_not_have_ExcludedFromCodeCoverage_attribute : ArchRule
    {
        internal override void Check()
        {
            var result = NotificationHandlers
                .ShouldNot()
                .HaveCustomAttribute(typeof(ExcludeFromCodeCoverageAttribute))
                .GetResult();

            AssertArchTestResult(result);
        }
    }
}
