using System.Diagnostics.CodeAnalysis;

namespace DAYA.ArchRules.Application.Commands.Handlers
{
    class CommandHandlers_can_not_have_ExcludedFromCodeCoverage_attribute : ArchRule
    {
        internal override void Check()
        {
            var result = CommandHandlers
                .ShouldNot()
                .HaveCustomAttribute(typeof(ExcludeFromCodeCoverageAttribute))
                .GetResult();

            AssertArchTestResult(result);
        }
    }
}
