using DAYA.ArchRules.Utilities;
using System.Reflection;

namespace DAYA.ArchRules.Application.Commands.Handlers
{
    class CommandHandlerCanNotHavePublicFields : ArchRule
    {
        internal override void Check()
        {
            const BindingFlags bindingFlags = BindingFlags.DeclaredOnly |
                                              BindingFlags.Public |
                                              BindingFlags.Instance;

            var result = CommandHandlers
                .ShouldNot()
                .HaveField(bindingFlags);

            AssertArchTestResult(result);
        }
    }
}
