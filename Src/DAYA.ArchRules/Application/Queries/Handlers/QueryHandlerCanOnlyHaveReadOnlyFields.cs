using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DAYA.ArchRules.Application.Queries.Handlers
{
    class QueryHandlerCanOnlyHaveReadOnlyFields : ArchRule
    {
        internal override void Check()
        {
            var types = QueryHandlersTypes;

            const BindingFlags bindingFlags = BindingFlags.DeclaredOnly |
                                              BindingFlags.NonPublic |
                                              BindingFlags.Public |
                                              BindingFlags.Instance;

            var failingTypes = new List<Type>();
            foreach (var type in types)
            {
                if (type.GetFields(bindingFlags).Any(x => !x.IsInitOnly))
                {
                    failingTypes.Add(type);
                }
            }

            AssertFailingTypes(failingTypes);
        }
    }
}
