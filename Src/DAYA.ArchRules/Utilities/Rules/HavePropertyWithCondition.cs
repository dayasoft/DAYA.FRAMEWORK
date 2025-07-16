using System;
using System.Reflection;
using Mono.Cecil;
using NetArchTest.Rules;

namespace DAYA.ArchRules.Utilities.Rules
{
    internal class HavePropertyWithCondition : ICustomRule
    {
        private readonly BindingFlags _bindingFlags;
        private readonly Func<PropertyInfo, bool> _condition;

        public HavePropertyWithCondition(BindingFlags bindingFlags, Func<PropertyInfo, bool> condition)
        {
            _bindingFlags = bindingFlags;
            _condition = condition;
        }

        public bool MeetsRule(TypeDefinition typeDefinition)
        {
            var assemblyName = typeDefinition.Module.Assembly.Name.Name;
            var assembly = Assembly.Load(assemblyName);
            var type = assembly.GetType(typeDefinition.FullName);

            return type.HavePropertyWithCondition(_bindingFlags, _condition);
        }
    }
}
