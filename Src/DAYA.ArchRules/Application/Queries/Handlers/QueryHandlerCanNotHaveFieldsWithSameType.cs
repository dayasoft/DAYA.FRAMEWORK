using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DAYA.ArchRules.Application.Queries.Handlers
{
    class QueryHandlerCanNotHaveFieldsWithSameType : ArchRule
    {
        internal override void Check()
        {
            var types = CommandHandlersTypes;

            const BindingFlags bindingFlags = BindingFlags.DeclaredOnly |
                                              BindingFlags.NonPublic |
                                              BindingFlags.Public |
                                              BindingFlags.Instance;

            var failingTypes = new List<Type>();
            foreach (var type in types)
            {
                if (type.GetFields(bindingFlags)
                    .GroupBy(x => x.FieldType)
                    .Any(x => x.Count() > 1))
                {
                    failingTypes.Add(type);
                }
            }

            AssertFailingTypes(failingTypes);
        }
    }
}
