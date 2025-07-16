using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DAYA.Cloud.Framework.V2.DirectOperations;
using DAYA.Cloud.Framework.V2.Domain;
using NetArchTest.Rules;

namespace DAYA.ArchRules.Domain
{
    internal class AggregateKeyNameMustBeIdOrEntityId : ArchRule
    {
        internal override void Check()
        {
            var entityTypes = Types.InAssembly(Data.DomainAssembly)
               .That()
               .Inherit(typeof(Entity))
               .And().Inherit(typeof(CosmosEntity)).GetTypes();

            var complexKeys = Types.InAssembly(Data.DomainAssembly)
                .That().Inherit(typeof(TypedId<>)).GetTypes().ToList();

            const BindingFlags bindingFlags = BindingFlags.DeclaredOnly |
                                              BindingFlags.NonPublic |
                                              BindingFlags.Public |
                                              BindingFlags.Instance;

            var failingTypes = new List<Type>();
            foreach (var type in entityTypes)
            {
                var complexKey = complexKeys.Where(x => x.Name == type.Name + "Id").FirstOrDefault();

                if (complexKey == null)
                {
                    failingTypes.Add(type);
                    break;
                }

                var properties = type.GetProperties(bindingFlags);
                int count = properties.Where(x => type.Name + "Id" == x.Name || "Id" == x.Name).Count();

                if (count != 1)
                {
                    failingTypes.Add(type);
                    break;
                }
            }

            AssertFailingTypes(failingTypes);
        }
    }
}