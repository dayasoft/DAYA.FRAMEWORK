using DAYA.Cloud.Framework.V2.Infrastructure.AzureSearch;
using NetArchTest.Rules;

namespace DAYA.ArchRules.Application.Commands.Search
{
    internal class SearchIndexCreators_should_not_be_public : ArchRule
    {
        internal override void Check()
        {
            var result = Types.InAssembly(Data.ApplicationAssembly)
                .That()
                .ImplementInterface(typeof(ISearchIndexCreator<,>))
                .ShouldNot()
                .BePublic();

            AssertArchTestResult(result);
        }
    }
}