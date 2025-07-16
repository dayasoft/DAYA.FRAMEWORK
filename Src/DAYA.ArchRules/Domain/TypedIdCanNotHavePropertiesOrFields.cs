using DAYA.Cloud.Framework.V2.Domain;
using NetArchTest.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DAYA.ArchRules.Domain
{
    internal class TypedIdCanNotHavePropertiesOrFields : ArchRule
    {
        internal override void Check()
        {
            var complexKeyTypes = Types.InAssembly(Data.DomainAssembly)
               .That().Inherit(typeof(TypedId<>)).GetTypes().ToList();

            const BindingFlags bindingFlags = BindingFlags.DeclaredOnly |
                                              BindingFlags.NonPublic |
                                              BindingFlags.Public |
                                              BindingFlags.Instance;

            var failingTypes = new List<Type>();
            foreach (var type in complexKeyTypes)
            {
                var fields = type.GetFields(bindingFlags);
                var properties = type.GetProperties(bindingFlags);
                if (fields.Length != 0 || properties.Length != 0)
                {
                    failingTypes.Add(type);
                }
            }

            AssertFailingTypes(failingTypes);
        }
    }
}