using System.Linq;
using FluentValidation;
using NetArchTest.Rules;

namespace DAYA.ArchRules.Application.Commands.Commands
{
    class CommandValidatorShouldHaveNameEndingWithCommandValidator : ArchRule
    {
        internal override void Check()
        {
            var commandTypes = Commands.GetTypes();

            var types = Types.InAssembly(Data.ApplicationAssembly)
                .That()
                .Inherit(typeof(AbstractValidator<>))
                .GetTypes()
                .Where(x => commandTypes.Any(y => x.BaseType
                    .GenericTypeArguments
                    .First()
                    .IsAssignableFrom(y)))
                .Where(x => !x.Name.EndsWith("CommandValidator"));

            AssertFailingTypes(types);
        }
    }
}
