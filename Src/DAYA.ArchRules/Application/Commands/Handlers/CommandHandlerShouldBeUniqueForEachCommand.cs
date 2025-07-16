using System.Linq;

namespace DAYA.ArchRules.Application.Commands.Handlers
{
    internal class CommandHandlerShouldBeUniqueForEachCommand : ArchRule
    {
        internal override void Check()
        {
            var allCommands = Commands.GetTypes();
            var handlers = CommandHandlersTypes.ToArray();

            var failingTypes = handlers
                .Select(x => x.GetInterfaces()
                    .First(x => x.Name.Contains("ICommandHandler") || x.Name.Contains("IDirectCommandHandler"))
                    .GenericTypeArguments.First())
                .GroupBy(x => x)
                .Where(g => g.Count() > 1)
                .Select(y => y.Key);

            AssertFailingTypes(failingTypes);
        }
    }
}