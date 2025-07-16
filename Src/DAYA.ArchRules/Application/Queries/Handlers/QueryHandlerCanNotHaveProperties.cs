using System;
using System.Collections.Generic;
using System.Linq;

namespace DAYA.ArchRules.Application.Queries.Handlers
{
    class QueryHandlerCanNotHaveProperties : ArchRule
    {
        internal override void Check()
        {
            var types = QueryHandlersTypes;

            var failingTypes = new List<Type>();
            foreach (var type in types)
            {
                var properties = type.GetProperties();
                if (properties.Any())
                {
                    failingTypes.Add(type);
                    break;
                }
            }

            AssertFailingTypes(failingTypes);
        }
    }
}
