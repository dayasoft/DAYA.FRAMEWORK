using System.Linq;
using DAYA.ArchRules.Utilities;
using DAYA.Cloud.Framework.V2.Domain;
using NetArchTest.Rules;

namespace DAYA.ArchRules.Application.Commands.Commands
{
    internal class CommandsShouldOnlyHavePrimitiveOrDefinedTypeProperties : ArchRule
    {
        internal override void Check()
        {
            var definedTypes = ApplicationDefinedTypes;
            var complexKeys = Types.InAssembly(Data.DomainAssembly)
                .That().Inherit(typeof(TypedId<>)).GetTypes().ToList();
            definedTypes = definedTypes
                .Union(complexKeys)
                .Union(ValueObjects.GetTypes())
                .ToHashSet();

            var result = Commands
                .Should()
                .HaveOnlySimpleOrDefinedProperties(definedTypes);

            AssertArchTestResult(result);
        }
    }
}