using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DAYA.Cloud.Framework.V2.DirectOperations;
using DAYA.Cloud.Framework.V2.Domain;
using NetArchTest.Rules;

namespace DAYA.ArchRules.Domain
{
    internal class AggregateMustHaveAPropertyWhichIsItsComplexKey : ArchRule
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

                var fields = type.GetFields(bindingFlags);
                int count = fields.Where(x => complexKey == x.FieldType).Count();

                if (count < 1)
                {
                    failingTypes.Add(type);
                    break;
                }

                var properties = type.GetProperties(bindingFlags);
                count = properties.Where(x => complexKey == x.PropertyType).Count();

                if (count < 1)
                {
                    failingTypes.Add(type);
                    break;
                }
            }

            AssertFailingTypes(failingTypes);
        }
    }
}