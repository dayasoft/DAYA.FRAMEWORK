using System;
using System.Collections.Generic;
using System.Reflection;
using Mono.Cecil;
using NetArchTest.Rules;

namespace DAYA.ArchRules.Utilities.Rules
{
    internal class HaveComplexPropertiesAndFieldsThatInheritDefiendTypesRule : ICustomRule
    {
        private readonly BindingFlags _bindingFlags;
        private readonly List<Type> _definedTypes;

        public HaveComplexPropertiesAndFieldsThatInheritDefiendTypesRule(List<Type> definedTypes, BindingFlags bindingFlags)
        {
            _definedTypes = definedTypes;
            _bindingFlags = bindingFlags;
        }

        public bool MeetsRule(TypeDefinition typeDefinition)
        {
            var assemblyName = typeDefinition.Module.Assembly.Name.Name;
            var assembly = Assembly.Load(assemblyName);
            var type = assembly.GetType(typeDefinition.FullName);

            return type.ComplexPropertiesAndFieldsInheritDefiendTypes(_definedTypes, _bindingFlags);
        }
    }
}
