using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DAYA.ArchRules.Domain.ValueObjectRules
{
    class ValueObjects_can_not_have_array_property : ArchRule
    {
        internal override void Check()
        {
            var types = ValueObjects.GetTypes();

            var failingTypes = new List<Type>();
            foreach (var type in types)
            {
                var properties = type
                    .GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
                    .Where(x =>
                        x.PropertyType.IsArray);
                if (properties.Any())
                {
                    failingTypes.Add(type);
                }
            }

            AssertFailingTypes(failingTypes);
        }
    }
}
