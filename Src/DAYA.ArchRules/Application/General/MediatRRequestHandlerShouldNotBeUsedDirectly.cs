using System;
using System.Collections.Generic;
using System.Linq;
using DAYA.Cloud.Framework.V2.Application.Configuration.Commands;
using DAYA.Cloud.Framework.V2.Application.Configuration.Notifications;
using DAYA.Cloud.Framework.V2.Application.Configuration.Queries;
using DAYA.Cloud.Framework.V2.DirectOperations.Contracts;
using MediatR;
using NetArchTest.Rules;

namespace DAYA.ArchRules.Application.General
{
    internal class MediatRRequestHandlerShouldNotBeUsedDirectly : ArchRule
    {
        internal override void Check()
        {
            var types = Types.InAssembly(Data.ApplicationAssembly)
                .That().DoNotHaveName("ICommandHandler`1").Or().DoNotHaveName("IDirectCommandHandler`1")
                .Should().ImplementInterface(typeof(IRequestHandler<>))
                .GetTypes();

            var failingTypes = new List<Type>();
            foreach (var type in types)
            {
                var isCommandHandler = type.GetInterfaces().Any(x =>
                    x.IsGenericType &&
                    (x.GetGenericTypeDefinition() == typeof(ICommandHandler<>) || x.GetGenericTypeDefinition() == typeof(IDirectCommandHandler<>)));

                var isQueryHandler = type.GetInterfaces().Any(x =>
                    x.IsGenericType &&
                    x.GetGenericTypeDefinition() == typeof(IQueryHandler<,>));

                var isNotificationHandler = type.GetInterfaces().Any(x =>
                    x.IsGenericType &&
                    x.GetGenericTypeDefinition() == typeof(IDomainNotificationHandler<>));

                if (!(isCommandHandler || isQueryHandler || isNotificationHandler))
                {
                    failingTypes.Add(type);
                }
            }

            AssertFailingTypes(failingTypes);
        }
    }
}