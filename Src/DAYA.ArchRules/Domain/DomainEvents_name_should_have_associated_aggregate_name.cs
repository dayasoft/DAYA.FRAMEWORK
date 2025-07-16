using System;
using System.Collections.Generic;
using System.Linq;
using DAYA.Cloud.Framework.V2.DirectOperations;
using DAYA.Cloud.Framework.V2.Domain;
using NetArchTest.Rules;

namespace DAYA.ArchRules.Domain
{
    internal class DomainEvents_name_should_have_associated_aggregate_name : ArchRule
    {
        internal override void Check()
        {
            var failingTypes = new List<Type>();

            var domainEvents = Types.InAssembly(Data.DomainAssembly)
                .That()
                .Inherit(typeof(DomainEventBase))
                .Or()
                .Inherit(typeof(IDomainEvent))
                .GetTypes();

            var aggregateNames = Entities
                .And()
                .Inherit(typeof(CosmosEntity))
                .GetTypes()
                .Select(x => x.Name);

            foreach (var domainEvent in domainEvents)
            {
                if (!aggregateNames.Any(x => domainEvent.Name.Contains(x)))
                {
                    failingTypes.Add(domainEvent);
                }
            }

            AssertFailingTypes(failingTypes);
        }
    }
}