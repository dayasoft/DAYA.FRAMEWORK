using System.Linq;
using System.Reflection;
using DAYA.ArchRules.Utilities;

namespace DAYA.ArchRules.Domain.ValueObjectRules
{
    class ValueObjects_complex_fields_should_be_TypedId_or_valueObject : ArchRule
    {
        internal override void Check()
        {
            const BindingFlags bindingFlags = BindingFlags.DeclaredOnly |
                                              BindingFlags.NonPublic |
                                              BindingFlags.Public |
                                              BindingFlags.Instance;

            var result = ValueObjects
               .Should()
               .HaveComplexPropertiesAndFieldsThatInheritDefiendTypes(
                    ValueObjects.GetTypes()
                    .Union(TypedIds.GetTypes()).ToList(), bindingFlags);

            AssertArchTestResult(result);
        }
    }
}
