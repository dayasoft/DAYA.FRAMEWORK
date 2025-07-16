namespace DAYA.ArchRules.Application.Commands.Commands
{
    class CommandShouldBePublic : ArchRule
    {
        internal override void Check()
        {
            var result = Commands
                .Should()
                .BePublic()
                .GetResult();

            AssertArchTestResult(result);
        }
    }
}
