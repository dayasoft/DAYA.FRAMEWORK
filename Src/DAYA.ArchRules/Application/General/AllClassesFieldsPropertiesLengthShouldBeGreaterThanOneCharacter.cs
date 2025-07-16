using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NetArchTest.Rules;

namespace DAYA.ArchRules.Application.General
{
    class AllClassesFieldsPropertiesLengthShouldBeGreaterThanOneCharacter : ArchRule
    {
        internal override void Check()
        {
            var domainObjectTypes = Types.InAssembly(Data.ApplicationAssembly)
                .GetTypes();

            var failingTypes = new List<Type>();
            foreach (var domainObjectType in domainObjectTypes)
            {
                var privateFields =
                    domainObjectType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                        .Where(x => x.Name.Length == 1);
                if (privateFields.Any())
                {
                    failingTypes.Add(domainObjectType);
                }

                var properties =
                    domainObjectType.GetProperties().Where(x => x.Name.Length == 1);

                if (properties.Any())
                {
                    failingTypes.Add(domainObjectType);
                }
            }

            AssertFailingTypes(failingTypes);
        }
    }
}
