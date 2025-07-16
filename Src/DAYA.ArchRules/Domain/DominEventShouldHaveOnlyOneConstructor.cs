using DAYA.Cloud.Framework.V2.Domain;
using NetArchTest.Rules;
using System;
using System.Collections.Generic;

namespace DAYA.ArchRules.Domain
{
    internal class DominEventShouldHaveOnlyOneConstructor : ArchRule
    {
        internal override void Check()
        {
            var domainEvents = Types.InAssembly(Data.DomainAssembly)
                .That()
                .Inherit(typeof(DomainEventBase)).GetTypes();

            var failingTypes = new List<Type>();
            foreach (var domainEvent in domainEvents)
            {
                if (domainEvent.GetConstructors().Length != 1)
                {
                    failingTypes.Add(domainEvent);
                }
            }

            AssertFailingTypes(failingTypes);
        }
    }
}