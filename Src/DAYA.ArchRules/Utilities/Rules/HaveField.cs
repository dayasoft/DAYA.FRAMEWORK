using Mono.Cecil;
using NetArchTest.Rules;
using System.Reflection;

namespace DAYA.ArchRules.Utilities.Rules
{
    internal class HaveField : ICustomRule
    {
        private readonly BindingFlags _bindingFlags;

        public HaveField(BindingFlags bindingFlags)
        {
            _bindingFlags = bindingFlags;
        }

        public bool MeetsRule(TypeDefinition typeDefinition)
        {
            var assemblyName = typeDefinition.Module.Assembly.Name.Name;
            var assembly = Assembly.Load(assemblyName);
            var type = assembly.GetType(typeDefinition.FullName);

            return type.HaveField(_bindingFlags);
        }
    }
}
