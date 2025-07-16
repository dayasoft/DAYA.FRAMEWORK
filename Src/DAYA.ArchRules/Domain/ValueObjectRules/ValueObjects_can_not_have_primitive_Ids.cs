using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DAYA.Cloud.Framework.V2.Domain;

namespace DAYA.ArchRules.Domain.ValueObjectRules
{
    internal class ValueObjects_can_not_have_primitive_Ids : ArchRule
    {
        internal override void Check()
        {
            var valueObjects = ValueObjects.GetTypes();

            var flags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance;
            var failingTypes = new List<Type>();
            foreach (var valueObject in valueObjects)
            {
                var properties = valueObject.GetProperties(flags)
                    .Where(x => x.Name.EndsWith("Id") && !x.PropertyType.IsAssignableTo(typeof(TypedId)));
                if (properties.Any())
                {
                    failingTypes.Add(valueObject);
                }
            }

            AssertFailingTypes(failingTypes);
        }
    }
}