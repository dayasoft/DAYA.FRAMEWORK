using DAYA.Cloud.Framework.V2.Application.Contracts;
using DAYA.Cloud.Framework.V2.Infrastructure.AzureSearch;
using NetArchTest.Rules;
using System.Linq;

namespace DAYA.ArchRules.Application.Queries.Search
{
    internal class SearchOptionFactory_should_exists_for_each_search_query : ArchRule
    {
        internal override void Check()
        {
            var allSearchQueries = Types.InAssembly(Data.ApplicationAssembly)
                .That()
                .Inherit(typeof(SearchQuery<>))
                .GetTypes();
            var handlers = Types.InAssembly(Data.ApplicationAssembly)
                .That()
                .ImplementInterface(typeof(ISearchOptionFactory<,>))
                .GetTypes()
                .ToArray();

            var queriesWhichHaveHandlers = handlers
                .Select(x => x.GetInterfaces()
                    .First(x => x.Name.Contains("ISearchOptionFactory"))
                    .GenericTypeArguments.First())
                .ToList();

            AssertFailingTypes(allSearchQueries.Except(queriesWhichHaveHandlers));
        }
    }
}