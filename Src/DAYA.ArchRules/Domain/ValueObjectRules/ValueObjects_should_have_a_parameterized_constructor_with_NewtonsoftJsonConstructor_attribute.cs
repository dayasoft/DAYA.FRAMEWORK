using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DAYA.ArchRules.Domain.ValueObjectRules
{
    class ValueObjects_should_have_a_parameterized_constructor_with_NewtonsoftJsonConstructor_attribute : ArchRule
    {
        internal override void Check()
        {
            var types = ValueObjects
                .GetTypes();

            var failingTypes = new List<Type>();

            foreach (var type in types)
            {
                var constructors = type.GetConstructors(
                    BindingFlags.Public | 
                    BindingFlags.Instance |
                    BindingFlags.NonPublic);
                var constructorsWithAttribute = 0;
                var hasParams = false;
                foreach (var constructorInfo in constructors)
                {
                    var jsonConstructorAttribute = constructorInfo.GetCustomAttributes(typeof(JsonConstructorAttribute), false);
                    if (jsonConstructorAttribute.Length > 0)
                    {
                        constructorsWithAttribute++;
                        hasParams = constructorInfo.GetParameters().Any();
                    }
                }

                if (constructorsWithAttribute != 1 || !hasParams)
                {
                    failingTypes.Add(type);
                }
            }

            AssertFailingTypes(failingTypes);
        }
    }
}
