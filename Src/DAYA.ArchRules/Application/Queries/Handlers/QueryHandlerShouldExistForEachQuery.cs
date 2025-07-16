using System.Linq;

namespace DAYA.ArchRules.Application.Queries.Handlers
{
    class QueryHandlerShouldExistForEachQuery : ArchRule
    {
        internal override void Check()
        {
            var allQueries = Queries.GetTypes();
            var handlers = QueryHandlersTypes.ToArray();

            var queriesWhichHaveHandlers = GetQueriesFromHandlers(handlers)
                .ToList();

            AssertFailingTypes(allQueries.Except(queriesWhichHaveHandlers));
        }
    }
}
