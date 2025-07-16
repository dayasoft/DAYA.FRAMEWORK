using System;
using System.Collections.Generic;

namespace DAYA.ArchRules.Application.Queries.Queries
{
    class QueryResultShouldHavePublicPropertySetters : ArchRule
    {
        internal override void Check()
        {
            var types = QueryResults;

            var failingTypes = new List<Type>();
            foreach (var type in types)
            {
                if (ContainsNonPublicPropertySetter(type))
                {
                    failingTypes.Add(type);
                }
            }

            AssertFailingTypes(failingTypes);
        }
    }
}
