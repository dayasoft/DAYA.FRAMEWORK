using DAYA.ArchRules.Utilities;
using System.Reflection;

namespace DAYA.ArchRules.Application.Commands.Commands
{
    class Commands_can_not_have_list_property : ArchRule
    {
        internal override void Check()
        {
            var result = Commands
                .ShouldNot()
                .HaveListProperty(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);

            AssertArchTestResult(result);
        }
    }
}
