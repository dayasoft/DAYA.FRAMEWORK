using DAYA.Cloud.Framework.V2.Infrastructure.EventBus;
using NetArchTest.Rules;

namespace DAYA.ArchRules.IntegrationEvents
{
    internal class IntegrationEventsShouldResideInNamespaceThatContainsIntegrationEvents : ArchRule
    {
        internal override void Check()
        {
            var result = Types.InAssemblies(new[] { Data.DomainAssembly, Data.ApplicationAssembly, Data.InfrastructureAssembly, Data.IntegrationEventsAssembly })
                .That()
                .Inherit(typeof(IntegrationEvent))
                .Should()
                .ResideInNamespaceMatching("([A-Z.a-z])\\w+.IntegrationEvents.([A-Z.a-z])\\w+");

            AssertArchTestResult(result);
        }

        internal override string Message => "Integration events should reside in namespace matching \"([A-Z.a-z])\\w+.IntegrationEvents.([A-Z.a-z])\\w+\" ";
    }
}