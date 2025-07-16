using DAYA.Cloud.Framework.V2.Domain;
using NetArchTest.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DAYA.ArchRules.Domain.ValueObjectRules
{
    internal class ValueObjects_can_not_have_reference_to_entity : ArchRule
    {
        internal override void Check()
        {
            var valueTypes = Types.InAssembly(Data.DomainAssembly)
                .That()
                .Inherit(typeof(ValueObject))
                .GetTypes();
            var entities = Types.InAssembly(Data.DomainAssembly)
                .That()
                .Inherit(typeof(Entity))
                .GetTypes();

            const BindingFlags bindingFlags = BindingFlags.DeclaredOnly |
                                              BindingFlags.NonPublic |
                                              BindingFlags.Public |
                                              BindingFlags.Instance;

            var failingTypes = new List<Type>();
            foreach (var type in valueTypes)
            {
                var fields = type.GetFields(bindingFlags);
                if (fields.Any(x => entities.Contains(x.FieldType)))
                {
                    failingTypes.Add(type);
                }
                var properties = type.GetProperties(bindingFlags);
                if (properties.All(x => entities.Contains(x.PropertyType)))
                {
                    failingTypes.Add(type);
                }
            }

            AssertFailingTypes(failingTypes);
        }
    }
}