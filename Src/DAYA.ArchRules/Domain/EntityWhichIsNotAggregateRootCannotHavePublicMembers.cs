using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DAYA.Cloud.Framework.V2.DirectOperations;
using DAYA.Cloud.Framework.V2.Domain;
using NetArchTest.Rules;

namespace DAYA.ArchRules.Domain
{
    internal class EntityWhichIsNotAggregateRootCannotHavePublicMembers : ArchRule
    {
        internal override void Check()
        {
            var types = Types.InAssembly(Data.DomainAssembly)
                .That()
                .Inherit(typeof(Entity))
                .And().DoNotInherit(typeof(CosmosEntity)).GetTypes();

            const BindingFlags bindingFlags = BindingFlags.DeclaredOnly |
                                              BindingFlags.Public |
                                              BindingFlags.Instance |
                                              BindingFlags.Static;

            var failingTypes = new List<Type>();
            foreach (var type in types)
            {
                var publicFields = type.GetFields(bindingFlags);
                var publicProperties = type.GetProperties(bindingFlags);
                var publicMethods = type.GetMethods(bindingFlags)
                    .Where(x => x.GetBaseDefinition().DeclaringType != typeof(object))
                    .ToArray();

                if (publicFields.Any() ||
                    publicProperties.Any() ||
                    publicMethods.Any())
                {
                    failingTypes.Add(type);
                }
            }

            AssertFailingTypes(failingTypes);
        }
    }
}