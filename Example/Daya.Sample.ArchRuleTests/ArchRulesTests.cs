using System.Reflection;
using DAYA.ArchRules;
using Xunit;

namespace Daya.Sample.Tests.ArchRules
{
    public class ArchRulesTests
    {
        [Fact]
        public void Arch_rules_should_be_followed()
        {
            var domainAssembly = Assembly.Load("Daya.Sample.Domain");
            var applicationAssembly = Assembly.Load("Daya.Sample.Application");
            var infrastructureAssembly = Assembly.Load("Daya.Sample.Infrastructure");
            var apiAssembly = Assembly.Load("Daya.Sample.API");
            var integrationEventsAssembly = Assembly.Load("Daya.Sample.IntegrationEvents");

            var engine = new RuleEngine(domainAssembly, applicationAssembly, infrastructureAssembly,
            apiAssembly, integrationEventsAssembly);

            engine.Check();
        }
    }
}