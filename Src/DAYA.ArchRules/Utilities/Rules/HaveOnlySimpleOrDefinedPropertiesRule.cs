using System;
using System.Collections.Generic;
using System.Reflection;
using Mono.Cecil;
using NetArchTest.Rules;

namespace DAYA.ArchRules.Utilities.Rules
{
    internal class HaveOnlySimpleOrDefinedPropertiesRule : ICustomRule
    {
        private readonly HashSet<Type> _definedTypes;

        public HaveOnlySimpleOrDefinedPropertiesRule(HashSet<Type> definedTypes)
        {
            _definedTypes = definedTypes;
        }

        public bool MeetsRule(TypeDefinition typeDefinition)
        {
            var assemblyName = typeDefinition.Module.Assembly.Name.Name;
            var assembly = Assembly.Load(assemblyName);
            var type = assembly.GetType(typeDefinition.FullName);

            return type.ContainsSimpleOrDefinedProperties(_definedTypes);
        }
    }

    internal class HaveAConstructorMatchesWithFieldsAndPropsNamesRule : ICustomRule
    {
        private readonly BindingFlags _bindingFlags;

        public HaveAConstructorMatchesWithFieldsAndPropsNamesRule(BindingFlags bindingFlags)
        {
            _bindingFlags = bindingFlags;
        }

        public bool MeetsRule(TypeDefinition typeDefinition)
        {
            var assemblyName = typeDefinition.Module.Assembly.Name.Name;
            var assembly = Assembly.Load(assemblyName);
            var type = assembly.GetType(typeDefinition.FullName);

            return type.HaveAConstructorMatchesWithFieldsAndPropsNames(_bindingFlags);
        }
    }
}