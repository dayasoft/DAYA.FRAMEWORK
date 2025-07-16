using DAYA.ArchRules.Utilities;
using System.Reflection;

namespace DAYA.ArchRules.Application.Commands.Handlers
{
    class CommandHandlerCanOnlyHaveReadOnlyFields : ArchRule
    {
        internal override void Check()
        {
            const BindingFlags bindingFlags = BindingFlags.DeclaredOnly |
                                              BindingFlags.NonPublic |
                                              BindingFlags.Public |
                                              BindingFlags.Instance;

            var result = CommandHandlers
                .Should()
                .HaveOnlyReadOnlyField(bindingFlags);

            AssertArchTestResult(result);
        }
    }
}
