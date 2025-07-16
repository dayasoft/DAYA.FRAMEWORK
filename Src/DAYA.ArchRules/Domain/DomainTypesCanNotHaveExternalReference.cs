using System;
using System.Collections.Generic;
using System.Linq;
using NetArchTest.Rules;

namespace DAYA.ArchRules.Domain
{
    internal class DomainTypesCanNotHaveExternalReference : ArchRule
    {
        // List of interface names to exclude from the rule
        private readonly string[] _excludedInterfaceNames = new string[]
        {
            "ISearchModel",
            "IFacetItem",
            "ISearchResult",
            "IFacetResult",
            "IInternalService"
            // Add more interfaces to exclude here
        };

        // Method to easily apply exclusions for interfaces
        private PredicateList ApplyInterfaceExclusions(PredicateList predicateList)
        {
            var result = predicateList;

            foreach (var interfaceName in _excludedInterfaceNames)
            {
                var interfaceType = Data.DomainAssembly.GetTypes().FirstOrDefault(x => x.Name == interfaceName);
                if (interfaceType is not null)
                {
                    result = result.And().DoNotImplementInterface(interfaceType);
                }
            }

            return result;
        }

        internal override void Check()
        {
            var domainTypeFilter = Types.InAssembly(Data.DomainAssembly).That().AreNotInterfaces();

            // Apply all interface exclusions in a single method call
            domainTypeFilter = ApplyInterfaceExclusions(domainTypeFilter);

            // Check for internalServicesInterface separately as it was done in the original code
            var internalServicesInterface = Data.DomainAssembly.GetTypes().FirstOrDefault(x => x.Name == "IInternalService");
            if (internalServicesInterface is not null)
            {
                domainTypeFilter = domainTypeFilter.And().DoNotImplementInterface(internalServicesInterface);
            }

            var domainTypes = domainTypeFilter.GetTypes();

            var failingTypes = new List<Type>();
            foreach (var type in domainTypes)
            {
                if (!ContainsSimpleOrDefinedPropertiesOrFields(type, domainTypes.ToHashSet()))
                {
                    failingTypes.Add(type);
                }
            }

            AssertFailingTypes(failingTypes);
        }
    }
}