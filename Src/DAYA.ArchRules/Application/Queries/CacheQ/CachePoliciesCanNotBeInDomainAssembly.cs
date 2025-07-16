using CacheQ;
using NetArchTest.Rules;

namespace DAYA.ArchRules.Application.Queries.CacheQ
{
    class CachePoliciesCanNotBeInDomainAssembly : ArchRule
    {
        internal override void Check()
        {
            var types = Types.InAssembly(Data.DomainAssembly)
                .That()
                .ImplementInterface(typeof(ICachePolicy<>))
                .GetTypes();

            AssertFailingTypes(types);
        }
    }
}
