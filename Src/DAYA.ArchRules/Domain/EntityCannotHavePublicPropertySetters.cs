using DAYA.Cloud.Framework.V2.Domain;
using NetArchTest.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DAYA.ArchRules.Domain
{
    internal class EntityCannotHavePublicPropertySetters : ArchRule
    {
        internal override void Check()
        {
            var types = Types.InAssembly(Data.DomainAssembly)
                .That()
                .Inherit(typeof(Entity))
                .GetTypes();

            const BindingFlags bindingFlags = BindingFlags.DeclaredOnly |
                                              BindingFlags.Public |
                                              BindingFlags.Instance |
                                              BindingFlags.Static;

            var failingTypes = new List<Type>();
            foreach (var type in types)
            {
                var publicProperties = type.GetProperties(bindingFlags);

                if (publicProperties.Where(x => x.GetSetMethod() != null).Any())
                {
                    failingTypes.Add(type);
                }
            }

            AssertFailingTypes(failingTypes);
        }
    }
}