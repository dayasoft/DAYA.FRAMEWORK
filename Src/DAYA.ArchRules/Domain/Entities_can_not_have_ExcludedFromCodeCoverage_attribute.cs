using System.Diagnostics.CodeAnalysis;

namespace DAYA.ArchRules.Domain
{
    class Entities_can_not_have_ExcludedFromCodeCoverage_attribute : ArchRule
    {
        internal override void Check()
        {
            var result = Entities
                .ShouldNot()
                .HaveCustomAttribute(typeof(ExcludeFromCodeCoverageAttribute))
                .GetResult();

            AssertArchTestResult(result);
        }
    }
}
