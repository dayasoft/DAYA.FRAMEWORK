using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DAYA.Cloud.Framework.V2.Domain;

namespace DAYA.ArchRules.Domain
{
    internal class DomainEventsCanNotHavePrimitiveIds : ArchRule
    {
        internal override void Check()
        {
            var domainEvents = DomainEvents.GetTypes();

            var flags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance;
            var failingTypes = new List<Type>();
            foreach (var domainEvent in domainEvents)
            {
                var properties = domainEvent.GetProperties(flags)
                    .Where(x => x.Name.EndsWith("Id") && !x.PropertyType.IsAssignableTo(typeof(TypedId)));
                if (properties.Any())
                {
                    failingTypes.Add(domainEvent);
                }
            }

            AssertFailingTypes(failingTypes);
        }
    }
}