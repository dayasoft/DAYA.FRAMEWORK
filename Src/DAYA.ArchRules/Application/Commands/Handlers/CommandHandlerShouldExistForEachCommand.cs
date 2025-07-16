using System.Linq;

namespace DAYA.ArchRules.Application.Commands.Handlers
{
    internal class CommandHandlerShouldExistForEachCommand : ArchRule
    {
        internal override void Check()
        {
            var allCommands = Commands.GetTypes();
            var handlers = CommandHandlersTypes.ToArray();

            var commandsWhichHaveHandlers = handlers
                .Select(x => x.GetInterfaces()
                    .First(x => x.Name.Contains("ICommandHandler") || x.Name.Contains("IDirectCommandHandler"))
                    .GenericTypeArguments.First())
                .ToList();

            AssertFailingTypes(allCommands.Except(commandsWhichHaveHandlers));
        }
    }
}