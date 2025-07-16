using System.Linq;
using DAYA.ArchRules.Utilities;

namespace DAYA.ArchRules.Application.Commands.Commands
{
    class CommandsWithResultShouldOnlyReturnPrimitiveTypes : ArchRule
    {
        internal override void Check()
        {
            var commandResults = CommandResults.ToArray();

            AssertFailingTypes(commandResults
                .Where(x => !x.IsPrimitive()));
        }
    }
}
