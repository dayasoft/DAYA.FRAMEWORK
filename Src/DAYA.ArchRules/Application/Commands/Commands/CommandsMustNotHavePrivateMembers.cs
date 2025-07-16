using DAYA.ArchRules.Utilities;
using System.Reflection;

namespace DAYA.ArchRules.Application.Commands.Commands
{
    class CommandsMustNotHavePrivateMembers : ArchRule
    {
        internal override void Check()
        {
            var flags = BindingFlags.NonPublic | BindingFlags.Instance;
            var result = Commands
                .ShouldNot()
                .HavePropertyMoreThan(flags, 1);

            AssertArchTestResult(result);
        }
    }
}