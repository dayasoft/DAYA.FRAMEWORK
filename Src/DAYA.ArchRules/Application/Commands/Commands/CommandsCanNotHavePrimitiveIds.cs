using DAYA.ArchRules.Utilities;
using DAYA.Cloud.Framework.V2.Domain;
using System.Reflection;

namespace DAYA.ArchRules.Application.Commands.Commands
{
    internal class CommandsCanNotHavePrimitiveIds : ArchRule
    {
        internal override void Check()
        {
            var flags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance;
            var result = Commands
                .ShouldNot()
                .HavePropertyMatchCondition(flags, x => x.Name.EndsWith("Id") && !x.PropertyType.IsAssignableTo(typeof(TypedId)));

            AssertArchTestResult(result);
        }
    }
}