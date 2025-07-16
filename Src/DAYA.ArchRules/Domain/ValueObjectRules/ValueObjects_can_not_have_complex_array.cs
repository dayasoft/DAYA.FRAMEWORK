using NetArchTest.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DAYA.ArchRules.Utilities;
using DAYA.Cloud.Framework.V2.Domain;

namespace DAYA.ArchRules.Domain.ValueObjectRules
{
    internal class ValueObjects_can_not_have_complex_array : ArchRule
    {
        internal override void Check()
        {
            var valueObjectTypes = Types.InAssembly(Data.DomainAssembly)
                .That().Inherit(typeof(ValueObject)).GetTypes().ToList();

            const BindingFlags bindingFlags = BindingFlags.DeclaredOnly |
                                              BindingFlags.NonPublic |
                                              BindingFlags.Public |
                                              BindingFlags.Instance;

            var failingTypes = new List<Type>();
            foreach (var type in valueObjectTypes)
            {
                var properties = type.GetProperties(bindingFlags);
                foreach (var property in properties)
                {
                    if (!property.PropertyType.IsSimple() &&
                        property.PropertyType.IsArray)
                    {
                        failingTypes.Add(type);
                    }
                }
            }

            AssertFailingTypes(failingTypes);
        }
    }
}