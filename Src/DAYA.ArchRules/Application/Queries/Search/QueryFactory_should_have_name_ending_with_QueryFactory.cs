using DAYA.Cloud.Framework.V2.Infrastructure.AzureSearch;
using NetArchTest.Rules;

namespace DAYA.ArchRules.Application.Queries.Search
{
    internal class QueryFactory_should_have_name_ending_with_QueryFactory : ArchRule
    {
        internal override void Check()
        {
            var result = Types.InAssembly(Data.ApplicationAssembly)
                .That()
                .ImplementInterface(typeof(IQueryFactory<>))
                .Should()
                .HaveNameEndingWith("QueryFactory")
                .GetResult();

            AssertArchTestResult(result);
        }
    }
}