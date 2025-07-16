using DAYA.Cloud.Framework.V2.Application.Contracts;
using DAYA.Cloud.Framework.V2.Infrastructure.AzureSearch;
using NetArchTest.Rules;
using System.Linq;

namespace DAYA.ArchRules.Application.Commands.Search
{
    internal class SearchIndexCreator_should_exists_for_each_update_search_command : ArchRule
    {
        internal override void Check()
        {
            var allSearchCommands = Types.InAssembly(Data.ApplicationAssembly)
                .That()
                .Inherit(typeof(UpdateSearchCommand<>))
                .GetTypes();
            var handlers = Types.InAssembly(Data.ApplicationAssembly)
                .That()
                .ImplementInterface(typeof(ISearchIndexCreator<,>))
                .GetTypes()
                .ToArray();

            var commandsWhichHaveHandlers = handlers
                .Select(x => x.GetInterfaces()
                    .First(x => x.Name.Contains("ISearchIndexCreator"))
                    .GenericTypeArguments.First())
                .ToList();

            AssertFailingTypes(allSearchCommands.Except(commandsWhichHaveHandlers));
        }
    }
}