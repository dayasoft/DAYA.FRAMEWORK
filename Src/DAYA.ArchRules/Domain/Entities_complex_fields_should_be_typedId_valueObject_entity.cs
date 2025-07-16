using DAYA.ArchRules.Utilities;
using DAYA.Cloud.Framework.V2.Domain;
using NetArchTest.Rules;
using System.Linq;
using System.Reflection;

namespace DAYA.ArchRules.Domain
{
    internal class Entities_complex_fields_should_be_typedId_valueObject_entity : ArchRule
    {
        internal override void Check()
        {
            var definedTypes = Types.InAssembly(Data.DomainAssembly)
               .That()
               .Inherit(typeof(Entity))
               .Or()
               .Inherit(typeof(ValueObject))
               .Or()
               .Inherit(typeof(TypedId<>))
               .GetTypes()
               .ToList();

            const BindingFlags bindingFlags = BindingFlags.DeclaredOnly |
                                              BindingFlags.NonPublic |
                                              BindingFlags.Public |
                                              BindingFlags.Instance;

            var result = Entities
               .Should()
               .HaveComplexPropertiesAndFieldsThatInheritDefiendTypes(definedTypes, bindingFlags);

            AssertArchTestResult(result);
        }
    }
}