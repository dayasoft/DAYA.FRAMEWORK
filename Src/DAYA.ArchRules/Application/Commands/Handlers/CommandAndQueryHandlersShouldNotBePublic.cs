using DAYA.Cloud.Framework.V2.Application.Configuration.Commands;
using DAYA.Cloud.Framework.V2.Application.Configuration.Queries;
using NetArchTest.Rules;

namespace DAYA.ArchRules.Application.Commands.Handlers
{
    internal class CommandAndQueryHandlersShouldNotBePublic : ArchRule
    {
        internal override void Check()
        {
            var types = Types.InAssembly(Data.ApplicationAssembly)
                .That()
                .ImplementInterface(typeof(IQueryHandler<,>))
                .Or()
                .ImplementInterface(typeof(ICommandHandler<,>))
                .Or()
                .ImplementInterface(typeof(ICommandHandler<>))
                .Should().NotBePublic().GetResult().FailingTypes;

            AssertFailingTypes(types);
        }
    }
}