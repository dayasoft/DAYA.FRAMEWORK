using DAYA.Cloud.Framework.V2.Domain;
using NetArchTest.Rules;

namespace DAYA.ArchRules.Domain
{
    internal class BusinessRuleShouldHaveRulePostfix : ArchRule
    {
        internal override void Check()
        {
            var result = Types.InAssembly(Data.DomainAssembly)
                .That()
                .ImplementInterface(typeof(IBusinessRule))
                .Should().HaveNameEndingWith("Rule")
                .GetResult();

            AssertArchTestResult(result);
        }
    }
}