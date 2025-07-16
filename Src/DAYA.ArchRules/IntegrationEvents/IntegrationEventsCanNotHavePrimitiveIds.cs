using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DAYA.Cloud.Framework.V2.Domain;

namespace DAYA.ArchRules.IntegrationEvents
{
    internal class IntegrationEventsCanNotHavePrimitiveIds : ArchRule
    {
        internal override void Check()
        {
            var integrationEvents = IntegrationEvents.GetTypes();

            var flags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance;
            var failingTypes = new List<Type>();
            foreach (var integrationEvent in integrationEvents)
            {
                var properties = integrationEvent.GetProperties(flags)
                    .Where(x => x.Name.EndsWith("Id") && !x.PropertyType.IsAssignableTo(typeof(TypedId)));
                if (properties.Any())
                {
                    failingTypes.Add(integrationEvent);
                }
            }

            AssertFailingTypes(failingTypes);
        }
    }
}