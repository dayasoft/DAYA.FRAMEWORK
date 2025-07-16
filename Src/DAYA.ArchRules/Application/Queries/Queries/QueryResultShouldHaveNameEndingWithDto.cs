using System;
using System.Linq;

namespace DAYA.ArchRules.Application.Queries.Queries
{
    internal class QueryResultShouldHaveNameEndingWithDto : ArchRule
    {
        internal override void Check()
        {
            var failingTypes = QueryResults
                .Where(x => !GetBaseTypeName(x).EndsWith("Dto"))
                .ToList();

            AssertFailingTypes(failingTypes);
        }

        private string GetBaseTypeName(Type type)
        {
            // Handle array types
            if (type.IsArray)
            {
                type = type.GetElementType(); // Get the element type of the array
            }

            // If the type is generic, extract the base name without generic arguments
            if (type.IsGenericType)
            {
                return type.Name.Substring(0, type.Name.IndexOf('`'));
            }

            return type.Name;
        }
    }
}