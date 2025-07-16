using System.Diagnostics.CodeAnalysis;

namespace DAYA.ArchRules.Application.Queries.Handlers
{
    class QueryHandlers_can_not_have_ExcludedFromCodeCoverage_attribute : ArchRule
    {
        internal override void Check()
        {
            var result = QueryHandlers
                .ShouldNot()
                .HaveCustomAttribute(typeof(ExcludeFromCodeCoverageAttribute))
                .GetResult();

            AssertArchTestResult(result);
        }
    }
}
