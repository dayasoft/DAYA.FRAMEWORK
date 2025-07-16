using System.Linq;
using System.Reflection;
using Mono.Cecil;
using NetArchTest.Rules;

namespace DAYA.ArchRules.Utilities.Rules
{
    internal class HavePropertyWithNameRule : ICustomRule
    {
        private static readonly BindingFlags _bindingFlags = BindingFlags.DeclaredOnly |
                                              BindingFlags.Public |
                                              BindingFlags.Instance |
                                              BindingFlags.NonPublic |
                                              BindingFlags.Static;

        private readonly string _name;

        public HavePropertyWithNameRule(string name)
        {
            _name = name;
        }

        public bool MeetsRule(TypeDefinition typeDefinition)
        {
            var assemblyName = typeDefinition.Module.Assembly.Name.Name;
            var assembly = Assembly.Load(assemblyName);
            var type = assembly.GetType(typeDefinition.FullName);

            var properties = type
                .GetProperties(_bindingFlags)
                .Where(x => x.Name.ToLower() == _name.ToLower());
            return properties.Any();
        }
    }
}