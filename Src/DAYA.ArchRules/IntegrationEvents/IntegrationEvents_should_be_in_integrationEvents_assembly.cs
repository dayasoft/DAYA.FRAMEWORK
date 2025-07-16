using DAYA.Cloud.Framework.V2.Infrastructure.EventBus;
using NetArchTest.Rules;

namespace DAYA.ArchRules.IntegrationEvents
{
    internal class IntegrationEvents_should_be_in_integrationEvents_assembly : ArchRule
    {
        internal override void Check()
        {
            var types = Types.InAssemblies(new[] { Data.DomainAssembly, Data.ApplicationAssembly, Data.InfrastructureAssembly })
                .That()
                .Inherit(typeof(IntegrationEvent))
                .GetTypes();

            AssertFailingTypes(types);
        }
    }
}