using CacheQ;
using NetArchTest.Rules;

namespace DAYA.ArchRules.Application.Queries.CacheQ
{
    class CachePoliciesCanNotBeInInfrastructureAssembly : ArchRule
    {
        internal override void Check()
        {
            var types = Types.InAssembly(Data.InfrastructureAssembly)
                .That()
                .ImplementInterface(typeof(ICachePolicy<>))
                .GetTypes();

            AssertFailingTypes(types);
        }
    }
}
