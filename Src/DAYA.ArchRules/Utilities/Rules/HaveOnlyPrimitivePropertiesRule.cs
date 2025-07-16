using System.Reflection;
using Mono.Cecil;
using NetArchTest.Rules;

namespace DAYA.ArchRules.Utilities.Rules
{
    internal class HaveOnlyPrimitivePropertiesRule : ICustomRule
    {
        public bool MeetsRule(TypeDefinition typeDefinition)
        {
            var assemblyName = typeDefinition.Module.Assembly.Name.Name;
            var assembly = Assembly.Load(assemblyName);
            var type = assembly.GetType(typeDefinition.FullName);

            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                if (!property.PropertyType.IsSimple())
                {
                    return false;
                }
            }
            return true;
        }
    }
}