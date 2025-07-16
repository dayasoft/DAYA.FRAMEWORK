using System.Linq;
using System.Reflection;
using Mono.Cecil;
using NetArchTest.Rules;

namespace DAYA.ArchRules.Utilities.Rules
{
    internal class HaveOnlyPublicPropertiesRule : ICustomRule
    {
        private static readonly BindingFlags _bindingFlags = BindingFlags.DeclaredOnly |
                                              BindingFlags.NonPublic |
                                              BindingFlags.Instance |
                                              BindingFlags.Static;

        public bool MeetsRule(TypeDefinition typeDefinition)
        {
            var assemblyName = typeDefinition.Module.Assembly.Name.Name;
            var assembly = Assembly.Load(assemblyName);
            var type = assembly.GetType(typeDefinition.FullName);

            var properties = type.GetProperties(_bindingFlags)
                .Where(x => x.Name != "EqualityContract");
            return !properties.Any();
        }
    }
}