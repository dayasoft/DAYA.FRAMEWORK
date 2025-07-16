using DAYA.ArchRules.Utilities;
using System.Reflection;

namespace DAYA.ArchRules.Application.Commands.Commands
{
    class CommandsCanNotHavePropertyWithNameId : ArchRule
    {
        internal override void Check()
        {
            var bindingFlags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance;

            var result = Commands
                .ShouldNot()
                .HavePropertyWithName(bindingFlags, "id");

            AssertArchTestResult(result);
        }
    }
}
