using Mono.Cecil;
using NetArchTest.Rules;
using System.Reflection;

namespace DAYA.ArchRules.Utilities.Rules
{
    internal class HaveDuplicateFieldType : ICustomRule
    {
        private readonly BindingFlags _bindingFlags;

        public HaveDuplicateFieldType(BindingFlags bindingFlags)
        {
            _bindingFlags = bindingFlags;
        }

        public bool MeetsRule(TypeDefinition typeDefinition)
        {
            var assemblyName = typeDefinition.Module.Assembly.Name.Name;
            var assembly = Assembly.Load(assemblyName);
            var type = assembly.GetType(typeDefinition.FullName);

            return type.HaveDuplicateFieldType(_bindingFlags);
        }
    }
}
