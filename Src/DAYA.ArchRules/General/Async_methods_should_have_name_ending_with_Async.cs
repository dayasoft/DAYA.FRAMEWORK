using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using DAYA.Cloud.Framework.V2.Application.Configuration.Commands;
using DAYA.Cloud.Framework.V2.Application.Configuration.Notifications;
using DAYA.Cloud.Framework.V2.Application.Configuration.Queries;
using DAYA.Cloud.Framework.V2.DirectOperations.Contracts;
using MediatR;
using NetArchTest.Rules;

namespace DAYA.ArchRules.General
{
    internal class Async_methods_should_have_name_ending_with_Async : ArchRule
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
                foreach (var method in type.GetMethods())
                {
                    if ((AsyncStateMachineAttribute)method.GetCustomAttribute(typeof(AsyncStateMachineAttribute)) != null)
                    {
                        if (type.GetInterfaces()
                                .Any(i => i.IsGenericType && (i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>) ||
                                                              i.GetGenericTypeDefinition() == typeof(ICommandHandler<>) ||
                                                              i.GetGenericTypeDefinition() == typeof(IDirectCommandHandler<>) ||
                                                              i.GetGenericTypeDefinition() == typeof(IDirectCommandHandler<,>) ||
                                                              i.GetGenericTypeDefinition() == typeof(IDomainNotificationHandler<>) ||
                                                              i.GetGenericTypeDefinition() == typeof(IPageableQueryHandler<,>) ||
                                                              i.GetGenericTypeDefinition() == typeof(INotificationHandler<>) ||
                                                              i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>))) && method.Name.Equals("Handle") ||
                            type.Name.EndsWith("Controller") ||
                            type.Name.EndsWith("Middleware"))
                        {
                            continue;
                        }

                        if (!method.Name.EndsWith("Async", StringComparison.Ordinal))
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