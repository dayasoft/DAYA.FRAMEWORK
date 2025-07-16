using NetArchTest.Rules;

namespace DAYA.ArchRules.Infrastructure.Audit
{
    internal class AuditConfigurationsShouldHaveAuditConfigurationPostfix : ArchRule
    {
        internal override void Check()
        {
            var result = Types.InAssembly(Data.InfrastructureAssembly)
                .That()
                .ImplementInterface(typeof(IAuditConfiguration<>))
                .Should()
                .HaveNameEndingWith("AuditConfiguration")
                .GetResult();

            AssertArchTestResult(result);
        }
    }
}