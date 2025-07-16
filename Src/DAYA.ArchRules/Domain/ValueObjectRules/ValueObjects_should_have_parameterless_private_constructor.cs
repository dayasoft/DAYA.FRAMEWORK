using DAYA.Cloud.Framework.V2.Domain;
using NetArchTest.Rules;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DAYA.ArchRules.Domain.ValueObjectRules
{
    internal class ValueObjects_should_have_parameterless_private_constructor : ArchRule
    {
        internal override void Check()
        {
            var entityTypes = Types.InAssembly(Data.DomainAssembly)
                .That()
                .Inherit(typeof(ValueObject)).GetTypes();

            var failingTypes = new List<Type>();
            foreach (var entityType in entityTypes)
            {
                var hasPrivateParameterlessConstructor = false;
                var constructors = entityType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (var constructorInfo in constructors)
                {
                    if (constructorInfo.IsPrivate && constructorInfo.GetParameters().Length == 0)
                    {
                        hasPrivateParameterlessConstructor = true;
                    }
                }

                if (!hasPrivateParameterlessConstructor)
                {
                    failingTypes.Add(entityType);
                }
            }

            AssertFailingTypes(failingTypes);
        }
    }
}