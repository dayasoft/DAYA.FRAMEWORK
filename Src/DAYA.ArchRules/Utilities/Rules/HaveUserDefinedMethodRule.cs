using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Mono.Cecil;
using NetArchTest.Rules;

namespace DAYA.ArchRules.Utilities.Rules
{
    internal class HaveUserDefinedMethodRule : ICustomRule
    {
        private static readonly BindingFlags _bindingFlags = BindingFlags.DeclaredOnly |
                                              BindingFlags.Public |
                                              BindingFlags.Instance |
                                              BindingFlags.NonPublic |
                                              BindingFlags.Static;

        public bool MeetsRule(TypeDefinition typeDefinition)
        {
            var assemblyName = typeDefinition.Module.Assembly.Name.Name;
            var assembly = Assembly.Load(assemblyName);
            var type = assembly.GetType(typeDefinition.FullName);

            var methods = type
                .GetMethods(_bindingFlags)
                .Where(x => x.CustomAttributes.All(attr => attr.AttributeType != typeof(CompilerGeneratedAttribute)));

            return methods.Any();
        }
    }
}