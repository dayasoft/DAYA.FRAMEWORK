using DAYA.Cloud.Framework.V2.Application.Configuration.Queries;
using NetArchTest.Rules;

namespace DAYA.ArchRules.Application.Queries.Handlers
{
    internal class QueryHandlerShouldHaveNameEndingWithQueryHandler : ArchRule
    {
        internal override void Check()
        {
            var result = Types.InAssembly(Data.ApplicationAssembly)
               .That()
               .ImplementInterface(typeof(IQueryHandler<,>))
               .Should()
               .HaveNameEndingWith("QueryHandler")
               .GetResult();

            AssertArchTestResult(result);
        }
    }
}