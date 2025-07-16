using CacheQ;
using NetArchTest.Rules;

namespace DAYA.ArchRules.Application.Queries.CacheQ
{
    class CachePoliciesShouldNotBePublic : ArchRule
    {
        internal override void Check()
        {
            var result = Types.InAssembly(Data.ApplicationAssembly)
                .That()
                .ImplementInterface(typeof(ICachePolicy<>))
                .ShouldNot()
                .BePublic();

            AssertArchTestResult(result);
        }
    }
}
