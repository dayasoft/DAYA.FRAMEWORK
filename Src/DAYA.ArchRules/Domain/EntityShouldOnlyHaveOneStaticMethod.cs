using DAYA.Cloud.Framework.V2.Domain;
using NetArchTest.Rules;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DAYA.ArchRules.Domain
{
    internal class EntityShouldOnlyHaveOneStaticMethod : ArchRule
    {
        internal override void Check()
        {
            var domainObjectTypes = Types.InAssembly(Data.DomainAssembly)
                .That()
                .Inherit(typeof(Entity))
                .GetTypes();

            var failingTypes = new List<Type>();
            foreach (var domainObjectType in domainObjectTypes)
            {
                var methods =
                    domainObjectType.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
                if (methods.Length > 1)
                {
                    failingTypes.Add(domainObjectType);
                }
            }

            AssertFailingTypes(failingTypes);
        }
    }
}