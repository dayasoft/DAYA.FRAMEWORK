using System.Reflection;
using Mono.Cecil;
using NetArchTest.Rules;

namespace DAYA.ArchRules.Utilities.Rules
{
    internal class HaveListProperty : ICustomRule
    {
        private readonly BindingFlags _bindingFlags;

        public HaveListProperty(BindingFlags bindingFlags)
        {
            _bindingFlags = bindingFlags;
        }

        public bool MeetsRule(TypeDefinition typeDefinition)
        {
            var assemblyName = typeDefinition.Module.Assembly.Name.Name;
            var assembly = Assembly.Load(assemblyName);
            var type = assembly.GetType(typeDefinition.FullName);

            return type.HaveListProperty(_bindingFlags);
        }
    }
}
