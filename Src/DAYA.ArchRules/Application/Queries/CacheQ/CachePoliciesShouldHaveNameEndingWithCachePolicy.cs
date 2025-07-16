using CacheQ;
using NetArchTest.Rules;

namespace DAYA.ArchRules.Application.Queries.CacheQ
{
    class CachePoliciesShouldHaveNameEndingWithCachePolicy : ArchRule
    {
        internal override void Check()
        {
            var result = Types.InAssembly(Data.ApplicationAssembly)
                .That()
                .ImplementInterface(typeof(ICachePolicy<>))
                .Should()
                .HaveNameEndingWith("CachePolicy");

            AssertArchTestResult(result);
        }
    }
}
