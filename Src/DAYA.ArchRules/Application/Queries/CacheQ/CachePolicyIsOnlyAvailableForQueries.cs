using System;
using System.Collections.Generic;
using System.Linq;
using CacheQ;
using NetArchTest.Rules;

namespace DAYA.ArchRules.Application.Queries.CacheQ
{
    class CachePolicyIsOnlyAvailableForQueries : ArchRule
    {
        internal override void Check()
        {
            var types = Types.InAssembly(Data.ApplicationAssembly)
                .That()
                .ImplementInterface(typeof(ICachePolicy<>))
                .GetTypes();

            var queries = Queries.GetTypes().ToHashSet();
            var failingTypes = new List<Type>();

            foreach (var type in types)
            {
                if (!queries.Contains(type.GetInterfaces()[0].GenericTypeArguments[0]))
                {
                    failingTypes.Add(type);
                }
            }

            AssertFailingTypes(failingTypes);
        }
    }
}
