using System;
using System.Collections.Generic;
using System.Linq;
using DAYA.Cloud.Framework.V2.DirectOperations;
using DAYA.Cloud.Framework.V2.Domain;
using NetArchTest.Rules;

namespace DAYA.ArchRules.Domain
{
    internal class TypedIdShouldExistForAggregateRootWithIdPostfix : ArchRule
    {
        internal override void Check()
        {
            var entityTypes = Types.InAssembly(Data.DomainAssembly)
                .That()
                .Inherit(typeof(Entity))
                .And().Inherit(typeof(CosmosEntity)).GetTypes();

            var complexKeys = Types.InAssembly(Data.DomainAssembly)
                .That().Inherit(typeof(TypedId<>)).GetTypes().ToList();

            var failingTypes = new List<Type>();
            foreach (var type in entityTypes)
            {
                if (complexKeys.Where(x => x.Name == type.Name + "Id").Count() != 1)
                {
                    failingTypes.Add(type);
                    break;
                }
            }

            AssertFailingTypes(failingTypes);
        }
    }
}