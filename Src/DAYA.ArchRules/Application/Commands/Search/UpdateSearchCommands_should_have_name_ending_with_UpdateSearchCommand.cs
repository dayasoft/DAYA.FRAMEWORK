using DAYA.Cloud.Framework.V2.Application.Contracts;
using NetArchTest.Rules;

namespace DAYA.ArchRules.Application.Commands.Search
{
    internal class UpdateSearchCommands_should_have_name_ending_with_UpdateSearchCommand : ArchRule
    {
        internal override void Check()
        {
            var result = Types.InAssembly(Data.ApplicationAssembly)
                .That()
                .Inherit(typeof(UpdateSearchCommand<>))
                .Should()
                .HaveNameEndingWith("UpdateSearchCommand");

            AssertArchTestResult(result);
        }
    }
}