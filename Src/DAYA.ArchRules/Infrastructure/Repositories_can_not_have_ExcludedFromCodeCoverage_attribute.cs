using System.Diagnostics.CodeAnalysis;
using DAYA.Cloud.Framework.V2.DirectOperations.Repositories;
using NetArchTest.Rules;

namespace DAYA.ArchRules.Infrastructure
{
    internal class Repositories_can_not_have_ExcludedFromCodeCoverage_attribute : ArchRule
    {
        internal override void Check()
        {
            var result = Types.InAssembly(Data.InfrastructureAssembly)
                .That()
                .ImplementInterface(typeof(ICosmosRepository<,>))
                .ShouldNot()
                .HaveCustomAttribute(typeof(ExcludeFromCodeCoverageAttribute))
                .GetResult();

            AssertArchTestResult(result);
        }
    }
}