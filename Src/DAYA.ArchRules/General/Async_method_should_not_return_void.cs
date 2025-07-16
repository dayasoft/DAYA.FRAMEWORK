using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using NetArchTest.Rules;

namespace DAYA.ArchRules.General
{
    class Async_method_should_not_return_void : ArchRule
    {
        internal override void Check()
        {
            var assemblies = new List<Assembly>
            {
                Data.DomainAssembly,
                Data.ApiAssembly,
                Data.InfrastructureAssembly,
                Data.ApplicationAssembly
            };

            var result = Types.InAssemblies(assemblies)
                .GetTypes();

            var failingTypes = new List<Type>();
            foreach (var type in result)
            {
                foreach (var method in type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic |
                                                       BindingFlags.Static   | BindingFlags.Public))
                {
                    if ((AsyncStateMachineAttribute)method.GetCustomAttribute(typeof(AsyncStateMachineAttribute)) !=
                        null)
                    {
                        if (method.ReturnType == typeof(void))
                        {
                            failingTypes.Add(type);
                            break;
                        }
                    }
                }
            }
            AssertFailingTypes(failingTypes);
        }
    }
}