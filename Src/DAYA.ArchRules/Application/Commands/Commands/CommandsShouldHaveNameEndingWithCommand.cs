namespace DAYA.ArchRules.Application.Commands.Commands
{
    class CommandsShouldHaveNameEndingWithCommand : ArchRule
    {
        internal override void Check()
        {
            var result = Commands
               .Should()
               .HaveNameEndingWith("Command")
               .GetResult();

            AssertArchTestResult(result);
        }
    }
}
