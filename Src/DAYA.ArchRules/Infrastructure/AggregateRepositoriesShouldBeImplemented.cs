using System;
using System.Collections.Generic;
using System.Linq;
using DAYA.Cloud.Framework.V2.DirectOperations.Repositories;
using NetArchTest.Rules;

namespace DAYA.ArchRules.Infrastructure
{
    internal class AggregateRepositoriesShouldBeImplemented : ArchRule
    {
        internal override void Check()
        {
            var interfaces = Types.InAssembly(Data.DomainAssembly)
                .That().ImplementInterface(typeof(ICosmosRepository<,>)).GetTypes().ToList();

            var failingTypes = new List<Type>();
            foreach (var type in interfaces)
            {
                if (Types.InAssembly(Data.InfrastructureAssembly)
                    .That()
                    .ImplementInterface(type)
                    .GetTypes().Count() != 1)
                {
                    failingTypes.Add(type);
                }
            }

            AssertFailingTypes(failingTypes);
        }
    }
}