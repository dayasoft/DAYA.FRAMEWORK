using DAYA.ArchRules.Utilities;

namespace DAYA.ArchRules.Application.Commands.Commands
{
    class CommandShouldBeImmutable : ArchRule
    {
        internal override void Check()
        {
            var result = Commands
                .Should()
                .BeInitOnly();

            AssertArchTestResult(result);
        }
    }
}