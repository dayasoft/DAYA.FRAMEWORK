using System;
using System.Collections.Generic;
using DAYA.ArchRules.Utilities;

namespace DAYA.ArchRules.Application.Queries.Queries
{
    class QueryResultPropertiesShouldBePrimitiveOrUserDefined : ArchRule
    {
        internal override void Check()
        {
            var types = QueryResults;

            var definedTypes = ApplicationDefinedTypes;

            definedTypes.UnionWith(ValueObjects.GetTypes());

            var failingTypes = new List<Type>();
            foreach (var type in types)
            {
                if (!type.ContainsSimpleOrDefinedProperties(definedTypes))
                {
                    failingTypes.Add(type);
                }
            }

            AssertFailingTypes(failingTypes);
        }
    }
}
