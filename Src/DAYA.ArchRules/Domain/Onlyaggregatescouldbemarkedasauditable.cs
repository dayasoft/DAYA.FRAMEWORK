using DAYA.Cloud.Framework.V2.DirectOperations;
using DAYA.Cloud.Framework.V2.Domain;
using NetArchTest.Rules;

namespace DAYA.ArchRules.Domain
{
    internal class Onlyaggregatescouldbemarkedasauditable : ArchRule
    {
        internal override void Check()
        {
            var result = Types.InAssembly(Data.DomainAssembly)
                .That()
                .ImplementInterface(typeof(IAuditable))
                .Should()
                .Inherit(typeof(CosmosEntity));

            AssertArchTestResult(result);
        }
    }
}