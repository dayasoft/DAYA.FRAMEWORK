using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using NetArchTest.Rules;

namespace DAYA.ArchRules.Utilities.Rules
{
    internal class UsedAsBaseClassRule : ICustomRule
    {
        private readonly IEnumerable<Type> _types;

        public UsedAsBaseClassRule(IEnumerable<Type> types)
        {
            _types = types;
        }

        public bool MeetsRule(TypeDefinition typeDefinition)
        {
            var assemblyName = typeDefinition.Module.Assembly.Name.Name;
            var assembly = Assembly.Load(assemblyName);
            var type = assembly.GetType(typeDefinition.FullName);

            var baseTypes = _types
                .Where(x => x.BaseType != null)
                .Select(x => x.BaseType);
            return baseTypes.Contains(type);
        }
    }
}