using DAYA.Cloud.Framework.V2.DirectOperations.Repositories;
using NetArchTest.Rules;

namespace DAYA.ArchRules.Infrastructure
{
    internal class ImplementedRepositoriesShouldHaveRepositoryPostfix : ArchRule
    {
        internal override void Check()
        {
            var result = Types.InAssembly(Data.InfrastructureAssembly)
                .That()
                .ImplementInterface(typeof(ICosmosRepository<,>))
                .Should()
                .HaveNameEndingWith("Repository")
                .GetResult();

            AssertArchTestResult(result);
        }
    }
}