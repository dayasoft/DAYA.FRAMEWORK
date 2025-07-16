using System.Linq;

namespace DAYA.ArchRules.Application.Queries.Handlers
{
    class QueryHandlerShouldBeUniqueForEachQuery : ArchRule
    {
        internal override void Check()
        {
            var allQueries = Queries.GetTypes();
            var handlers = QueryHandlersTypes.ToArray();

            var failingTypes = GetQueriesFromHandlers(handlers)
                .GroupBy(x => x)
                .Where(g => g.Count() > 1)
                .Select(y => y.Key);

            AssertFailingTypes(failingTypes);
        }
    }
}
