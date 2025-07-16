using System.Linq;
using System.Reflection;
using Mono.Cecil;
using NetArchTest.Rules;

namespace DAYA.ArchRules.Utilities.Rules
{
    internal class HaveAPublicConstructorRule : ICustomRule
    {
        private static readonly BindingFlags _bindingFlags = BindingFlags.DeclaredOnly |
                                          BindingFlags.Public |
                                          BindingFlags.Instance;

        public bool MeetsRule(TypeDefinition typeDefinition)
        {
            var assemblyName = typeDefinition.Module.Assembly.Name.Name;
            var assembly = Assembly.Load(assemblyName);
            var type = assembly.GetType(typeDefinition.FullName);

            var constructors = type.GetConstructors(_bindingFlags);

            return constructors.Any();
        }
    }
}