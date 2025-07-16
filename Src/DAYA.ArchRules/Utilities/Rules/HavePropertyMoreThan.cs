using Mono.Cecil;
using NetArchTest.Rules;
using System.Reflection;

namespace DAYA.ArchRules.Utilities.Rules
{
    internal class HavePropertyMoreThan : ICustomRule
    {
        private readonly BindingFlags _bindingFlags;
        private readonly int _max;

        public HavePropertyMoreThan(BindingFlags bindingFlags, int max)
        {
            _bindingFlags = bindingFlags;
            _max = max;
        }

        public bool MeetsRule(TypeDefinition typeDefinition)
        {
            var assemblyName = typeDefinition.Module.Assembly.Name.Name;
            var assembly = Assembly.Load(assemblyName);
            var type = assembly.GetType(typeDefinition.FullName);

            return type.HavePropertyMoreThan(_bindingFlags, _max);
        }
    }
}
