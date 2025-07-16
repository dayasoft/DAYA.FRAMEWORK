using System.Reflection;
using Mono.Cecil;
using NetArchTest.Rules;

namespace DAYA.ArchRules.Utilities.Rules
{
    internal class HavePropertyWithName : ICustomRule
    {
        private readonly BindingFlags _bindingFlags;
        private readonly string _name;

        public HavePropertyWithName(BindingFlags bindingFlags, string name)
        {
            _bindingFlags = bindingFlags;
            _name = name;
        }

        public bool MeetsRule(TypeDefinition typeDefinition)
        {
            var assemblyName = typeDefinition.Module.Assembly.Name.Name;
            var assembly = Assembly.Load(assemblyName);
            var type = assembly.GetType(typeDefinition.FullName);

            return type.HavePropertyWithName(_bindingFlags, _name);
        }
    }
}
