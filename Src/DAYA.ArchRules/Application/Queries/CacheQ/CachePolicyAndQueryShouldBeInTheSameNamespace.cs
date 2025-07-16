using System;
using System.Collections.Generic;
using CacheQ;
using NetArchTest.Rules;

namespace DAYA.ArchRules.Application.Queries.CacheQ
{
    class CachePolicyAndQueryShouldBeInTheSameNamespace : ArchRule
    {
        internal override void Check()
        {
            var types = Types.InAssembly(Data.ApplicationAssembly)
                .That()
                .ImplementInterface(typeof(ICachePolicy<>))
                .GetTypes();

            var failingTypes = new List<Type>();

            foreach (var type in types)
            {
                if (type.GetInterfaces()[0].GenericTypeArguments[0].Namespace != type.Namespace)
                {
                    failingTypes.Add(type);
                }
            }

            AssertFailingTypes(failingTypes);
        }
    }
}
