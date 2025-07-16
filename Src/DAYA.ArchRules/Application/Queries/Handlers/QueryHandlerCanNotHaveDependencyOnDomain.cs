using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NetArchTest.Rules;

namespace DAYA.ArchRules.Application.Queries.Handlers
{
    internal class QueryHandlerCanNotHaveDependencyOnDomain : ArchRule
    {
        internal override void Check()
        {
            var types = QueryHandlersTypes;

            var domainInterfaces = Types.InAssembly(Data.DomainAssembly)
                .GetTypes()
                .ToHashSet();

            const BindingFlags bindingFlags = BindingFlags.DeclaredOnly |
                                              BindingFlags.NonPublic |
                                              BindingFlags.Public |
                                              BindingFlags.Instance;

            var failingTypes = new List<Type>();

            var internalService = Data.DomainAssembly.GetTypes().FirstOrDefault(x => x.Name == "IInternalService");

            foreach (var type in types)
            {
                var typeFields = type.GetFields(bindingFlags)
                    .Where(x => x.FieldType.Name != "IAzureSearchService" && !ImplementsInterfaceByName(x.FieldType, "IInternalService"));

                if (typeFields.Any(x => domainInterfaces.Contains(x.FieldType)))
                {
                    failingTypes.Add(type);
                }
            }

            AssertFailingTypes(failingTypes);
        }

        private bool ImplementsInterfaceByName(Type fieldType, string interfaceName)
        {
            // Get all interfaces implemented by the field type
            var implementedInterfaces = fieldType.GetInterfaces();

            // Check if any of the implemented interface names match the given interface name
            return implementedInterfaces.Any(i => i.Name == interfaceName);
        }
    }
}