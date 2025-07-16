using MediatR;
using NetArchTest.Rules;

namespace DAYA.ArchRules.Application.General
{
    class INotificationHandlerImplementionsShouldHaveNotificationHandlerPostfix : ArchRule
    {
        internal override void Check()
        {
            var result = Types.InAssembly(Data.ApplicationAssembly)
                .That()
                .ImplementInterface(typeof(INotificationHandler<>))
                .Should()
                .HaveNameEndingWith("NotificationHandler")
                .GetResult();

            AssertArchTestResult(result);
        }
    }
}
