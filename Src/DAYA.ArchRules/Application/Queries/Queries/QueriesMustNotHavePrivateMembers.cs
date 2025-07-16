using DAYA.Cloud.Framework.V2.Application.Contracts;
using NetArchTest.Rules;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DAYA.ArchRules.Application.Queries.Queries
{
    internal class QueriesMustNotHavePrivateMembers : ArchRule
    {
        internal override void Check()
        {
            var types = Types.InAssembly(Data.ApplicationAssembly)
                .That().Inherit(typeof(Query<>))
                .GetTypes();

            var failingTypes = new List<Type>();

            foreach (var type in types)
            {
                var properties = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance);

                if (properties.Length > 1)
                {
                    failingTypes.Add(type);
                }
            }

            AssertFailingTypes(failingTypes);
        }
    }
}