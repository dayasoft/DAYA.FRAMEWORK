using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DAYA.Cloud.Framework.V2.DirectOperations;
using DAYA.Cloud.Framework.V2.Domain;
using NetArchTest.Rules;

namespace DAYA.ArchRules.Domain
{
    internal class AggregateCanNotHaveBothIdAndEntityIdAsPropertyOrField : ArchRule
    {
        internal override void Check()
        {
            var entityTypes = Types.InAssembly(Data.DomainAssembly)
                .That()
                .Inherit(typeof(Entity))
                .And().Inherit(typeof(CosmosEntity)).GetTypes();

            const BindingFlags bindingFlags = BindingFlags.DeclaredOnly |
                                              BindingFlags.NonPublic |
                                              BindingFlags.Public |
                                              BindingFlags.Instance;

            var failingTypes = new List<Type>();
            foreach (var type in entityTypes)
            {
                var fields = type.GetFields(bindingFlags);
                int count = fields.Where(x =>
                    x.Name.FieldName() == "Id" ||
                    x.Name.FieldName() == type.Name + "Id" ||
                    x.Name.FieldName() == "_id" ||
                    x.Name.FieldName() == "_" + type.Name + "Id")
                    .Count();

                if (count != 1)
                {
                    failingTypes.Add(type);
                    break;
                }

                var properties = type.GetProperties(bindingFlags);
                count = properties.Where(x =>
                    x.Name == "Id" ||
                    x.Name == type.Name + "Id" ||
                    x.Name == "_id" ||
                    x.Name == "_" + type.Name + "Id")
                    .Count();

                if (count != 1)
                {
                    failingTypes.Add(type);
                    break;
                }
            }

            AssertFailingTypes(failingTypes);
        }
    }
}