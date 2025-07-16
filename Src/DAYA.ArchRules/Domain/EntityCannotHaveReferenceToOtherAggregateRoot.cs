using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DAYA.Cloud.Framework.V2.DirectOperations;
using DAYA.Cloud.Framework.V2.Domain;
using NetArchTest.Rules;

namespace DAYA.ArchRules.Domain
{
    internal class EntityCannotHaveReferenceToOtherAggregateRoot : ArchRule
    {
        internal override void Check()
        {
            var entityTypes = Types.InAssembly(Data.DomainAssembly)
                .That()
                .Inherit(typeof(Entity)).GetTypes();

            var aggregateRoots = Types.InAssembly(Data.DomainAssembly)
                .That().Inherit(typeof(CosmosEntity)).GetTypes().ToList();

            const BindingFlags bindingFlags = BindingFlags.DeclaredOnly |
                                              BindingFlags.NonPublic |
                                              BindingFlags.Instance;

            var failingTypes = new List<Type>();
            foreach (var type in entityTypes)
            {
                var fields = type.GetFields(bindingFlags);

                foreach (var field in fields)
                {
                    if (aggregateRoots.Contains(field.FieldType) ||
                        field.FieldType.GenericTypeArguments.Any(x => aggregateRoots.Contains(x)))
                    {
                        failingTypes.Add(type);
                        break;
                    }
                }

                var properties = type.GetProperties(bindingFlags);
                foreach (var property in properties)
                {
                    if (aggregateRoots.Contains(property.PropertyType) ||
                        property.PropertyType.GenericTypeArguments.Any(x => aggregateRoots.Contains(x)))
                    {
                        failingTypes.Add(type);
                        break;
                    }
                }
            }

            AssertFailingTypes(failingTypes);
        }
    }
}