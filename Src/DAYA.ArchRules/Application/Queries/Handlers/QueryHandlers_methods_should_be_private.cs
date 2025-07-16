using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DAYA.ArchRules.Application.Queries.Handlers
{
    class QueryHandlers_methods_should_be_private : ArchRule
    {
        internal override void Check()
        {
            var types = QueryHandlersTypes;

            const BindingFlags bindingFlags = BindingFlags.DeclaredOnly |
                                              BindingFlags.Public |
                                              BindingFlags.NonPublic |
                                              BindingFlags.Instance;

            var failingTypes = new List<Type>();
            foreach (var type in types)
            {
                if (type.GetMethods(bindingFlags).Where(x => !x.IsPrivate && x.Name != "Handle").Any())
                {
                    failingTypes.Add(type);
                }
            }

            AssertFailingTypes(failingTypes);
        }
    }
}
