using System.Linq;

namespace DAYA.ArchRules.Application.Queries.Queries
{
    class QueryResultConnectionsShouldNotBeCyclic : ArchRule
    {
        internal override void Check()
        {
            var types = ApplicationDefinedTypes
                .ToList();

            var graph = BuildGraph(types);

            if (graph.IsCyclic())
            {
                throw new ArchitectureException("");
            }
        }
    }
}
