using System.Diagnostics.CodeAnalysis;

namespace DAYA.ArchRules.Domain.ValueObjectRules
{
    class ValueObjects_can_not_have_ExcludedFromCodeCoverage_attribute : ArchRule
    {
        internal override void Check()
        {
            var result = ValueObjects
                .ShouldNot()
                .HaveCustomAttribute(typeof(ExcludeFromCodeCoverageAttribute))
                .GetResult();

            AssertArchTestResult(result);
        }
    }
}
