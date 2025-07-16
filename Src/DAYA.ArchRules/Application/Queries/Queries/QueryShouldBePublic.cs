using DAYA.Cloud.Framework.V2.Application.Contracts;
using NetArchTest.Rules;

namespace DAYA.ArchRules.Application.Queries.Queries
{
    internal class QueryShouldBePublic : ArchRule
    {
        internal override void Check()
        {
            var result = Types.InAssembly(Data.ApplicationAssembly)
                .That()
                .Inherit(typeof(Query<>)).Or()
                .ImplementInterface(typeof(IQuery<>))
                .Should()
                .BePublic();

            AssertArchTestResult(result);
        }
    }
}