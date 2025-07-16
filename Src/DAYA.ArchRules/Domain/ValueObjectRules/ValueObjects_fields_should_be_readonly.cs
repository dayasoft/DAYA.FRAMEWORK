using DAYA.Cloud.Framework.V2.Domain;
using NetArchTest.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DAYA.ArchRules.Domain.ValueObjectRules
{
    internal class ValueObjects_fields_should_be_readonly : ArchRule
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
                var fields = type.GetFields(bindingFlags)
                    .Where(x => !x.IsInitOnly);
                if (fields.Any())
                {
                    failingTypes.Add(type);
                }
            }

            AssertFailingTypes(failingTypes);
        }
    }
}