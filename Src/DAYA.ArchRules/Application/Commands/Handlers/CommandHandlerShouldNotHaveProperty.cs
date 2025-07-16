using DAYA.ArchRules.Utilities;
using System.Reflection;

namespace DAYA.ArchRules.Application.Commands.Handlers
{
    class CommandHandlerShouldNotHaveProperty : ArchRule
    {
        internal override void Check()
        {
            var flags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public;
            var result = CommandHandlers
                .ShouldNot()
                .HavePropertyMoreThan(flags, 0);

            AssertArchTestResult(result);
        }
    }
}
