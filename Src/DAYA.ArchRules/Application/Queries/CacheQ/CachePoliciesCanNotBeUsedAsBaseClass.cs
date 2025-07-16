using CacheQ;
using DAYA.ArchRules.Utilities;
using NetArchTest.Rules;

namespace DAYA.ArchRules.Application.Queries.CacheQ
{
    class CachePoliciesCanNotBeUsedAsBaseClass : ArchRule
    {
        internal override void Check()
        {
            var result = Types.InAssembly(Data.ApplicationAssembly)
                .That()
                .ImplementInterface(typeof(ICachePolicy<>))
                .ShouldNot()
                .UsedAsBaseClass(AllTypes);

            AssertArchTestResult(result);
        }
    }
}
