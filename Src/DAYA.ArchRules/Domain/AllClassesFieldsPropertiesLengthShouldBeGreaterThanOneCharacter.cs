using NetArchTest.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DAYA.ArchRules.Domain
{
    class AllClassesFieldsPropertiesLengthShouldBeGreaterThanOneCharacter : ArchRule
    {
        internal override void Check()
        {
            var domainObjectTypes = Types.InAssembly(Data.DomainAssembly)
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
