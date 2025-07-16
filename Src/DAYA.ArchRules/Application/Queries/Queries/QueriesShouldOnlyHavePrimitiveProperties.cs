using System;
using System.Collections.Generic;

namespace DAYA.ArchRules.Application.Queries.Queries
{
    class QueriesShouldOnlyHavePrimitiveProperties : ArchRule
    {
        internal override void Check()
        {
            var types = Queries.GetTypes();

            var failingTypes = new List<Type>();
            foreach (var type in types)
            {
                var properties = type.GetProperties();
                foreach (var property in properties)
                {
                    if (!IsSimple(property.PropertyType))
                    {
                        failingTypes.Add(type);
                        break;
                    }
                }
            }

            AssertFailingTypes(failingTypes);
        }
    }
}
