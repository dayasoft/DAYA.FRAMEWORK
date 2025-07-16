using DAYA.Cloud.Framework.V2.Domain;
using NetArchTest.Rules;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAYA.ArchRules.Infrastructure.Audit
{
    internal class AuditConfigurationsShouldBeImplemented : ArchRule
    {
        internal override void Check()
        {
            var auditableAggregates = Types.InAssembly(Data.DomainAssembly)
                .That()
                .ImplementInterface(typeof(IAuditable))
                .GetTypes()
                .ToList();

            var auditConfigurations = Types.InAssembly(Data.InfrastructureAssembly)
                .That()
                .ImplementInterface(typeof(IAuditConfiguration<>))
                .GetTypes()
                .SelectMany(x => x.GetInterfaces().Where(x => x.IsGenericType && x.Name.Contains("IAuditConfiguration"))
                   .Select(x => x.GenericTypeArguments[0]))
                .ToList();

            var failingTypes = new List<Type>();
            foreach (var auditableAggregate in auditableAggregates)
            {
                if (!auditConfigurations.Contains(auditableAggregate))
                {
                    failingTypes.Add(auditableAggregate);
                }
            }

            AssertFailingTypes(failingTypes);
        }
    }
}