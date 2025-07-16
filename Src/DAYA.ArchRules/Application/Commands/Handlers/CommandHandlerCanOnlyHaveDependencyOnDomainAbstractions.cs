using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DAYA.ArchRules.Utilities;
using DAYA.Cloud.Framework.V2.Cosmos.Abstractions;
using DAYA.Cloud.Framework.V2.DirectOperations.Repositories;
using DAYA.Cloud.Framework.V2.Infrastructure.AzureSearch;
using NetArchTest.Rules;

namespace DAYA.ArchRules.Application.Commands.Handlers
{
    internal class CommandHandlerCanOnlyHaveDependencyOnDomainAbstractions : ArchRule
    {
        internal override void Check()
        {
            var types = CommandHandlersTypes
                .Where(x => !x.IsUpdateSearchCommandHandler());

            var domainInterfaces = Types.InAssembly(Data.DomainAssembly)
                .That()
                .AreInterfaces()
                .GetTypes()
                .ToHashSet();

            var applicationInterfaces = Types.InAssembly(Data.ApplicationAssembly)
                .That()
                .AreInterfaces()
                .GetTypes()
                .ToHashSet();

            var allowedTypes = domainInterfaces
                .Union(applicationInterfaces)
                .Union(new Type[] { typeof(ISearchIndexClientFactory), typeof(IContainerFactory) })
                .ToHashSet();
            const BindingFlags bindingFlags = BindingFlags.DeclaredOnly |
                                              BindingFlags.NonPublic |
                                              BindingFlags.Public |
                                              BindingFlags.Instance;

            var failingTypes = new List<Type>();
            foreach (var type in types)
            {
                var fields = type.GetFields(bindingFlags);
                if (!fields.All(x => allowedTypes.Contains(x.FieldType) || x.FieldType.IsGenericType && x.FieldType.GetGenericTypeDefinition() == typeof(ICosmosRepository<,>)))
                {
                    failingTypes.Add(type);
                }
            }

            AssertFailingTypes(failingTypes);
        }
    }
}