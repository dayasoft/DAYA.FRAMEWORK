using Mono.Cecil;
using NetArchTest.Rules;
using System.Linq;
using System.Reflection;

namespace DAYA.ArchRules.Utilities.Rules
{
    internal class HaveOnlyReadOnlyFieldRule : ICustomRule
    {
        private readonly BindingFlags _bindingFlags;

        public HaveOnlyReadOnlyFieldRule(BindingFlags bindingFlags)
        {
            _bindingFlags = bindingFlags;
        }

        public bool MeetsRule(TypeDefinition typeDefinition)
        {
            var assemblyName = typeDefinition.Module.Assembly.Name.Name;
            var assembly = Assembly.Load(assemblyName);
            var type = assembly.GetType(typeDefinition.FullName);

            return type.GetFields(_bindingFlags).All(x => x.IsInitOnly);
        }
    }
}