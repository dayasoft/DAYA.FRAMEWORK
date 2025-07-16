using DAYA.ArchRules.Utilities;
using System.Reflection;

namespace DAYA.ArchRules.Application.Commands.Handlers
{
    class CommandHandlerCanNotHaveFieldsWithSameType : ArchRule
    {
        internal override void Check()
        {
            const BindingFlags bindingFlags = BindingFlags.DeclaredOnly |
                                              BindingFlags.NonPublic |
                                              BindingFlags.Public |
                                              BindingFlags.Instance;

            var result = CommandHandlers
                .ShouldNot()
                .HaveDuplicateFieldType(bindingFlags);

            AssertArchTestResult(result);
        }
    }
}
