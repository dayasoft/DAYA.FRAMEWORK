using DAYA.Cloud.Framework.V2.Domain;
using NetArchTest.Rules;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DAYA.ArchRules.Domain
{
    internal class DomainObjectShouldHaveOnlyPrivateConstructors : ArchRule
    {
        internal override void Check()
        {
            var domainObjectTypes = Types.InAssembly(Data.DomainAssembly)
                .That()
                .Inherit(typeof(Entity))
                .Or()
                .Inherit(typeof(ValueObject))
                .GetTypes();

            var failingTypes = new List<Type>();
            foreach (var domainObjectType in domainObjectTypes)
            {
                var constructors =
                    domainObjectType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Public |
                                                     BindingFlags.Instance);
                foreach (var constructorInfo in constructors)
                {
                    if (!constructorInfo.IsPrivate)
                    {
                        failingTypes.Add(domainObjectType);
                    }
                }
            }

            AssertFailingTypes(failingTypes);
        }
    }
}