using DAYA.Cloud.Framework.V2.Domain;
using NetArchTest.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DAYA.ArchRules.Domain
{
    internal class Entity_Should_Contains_TypedId_In_FactoryMethod_Parameters : ArchRule
    {
        internal override void Check()
        {
            var entityTypes = Types.InAssembly(Data.DomainAssembly)
               .That()
               .Inherit(typeof(Entity))
               .GetTypes();

            var typedIds = Types.InAssembly(Data.DomainAssembly)
                .That().Inherit(typeof(TypedId<>)).GetTypes().ToList();

            var failingTypes = new List<Type>();
            foreach (var entityType in entityTypes)
            {
                var typedIdType = typedIds
                    .Where(x => x.Name == entityType.Name + "Id")
                    .Single();

                var containsTypedId = false;
                var factoryMethod = entityType.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
                    .Single();

                foreach (var parameter in factoryMethod.GetParameters())
                {
                    if (parameter.ParameterType == typedIdType)
                    {
                        containsTypedId = true;
                        break;
                    }
                }
                if (!containsTypedId)
                {
                    failingTypes.Add(entityType);
                    break;
                }
            }

            AssertFailingTypes(failingTypes);
        }
    }
}