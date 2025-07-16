using System.Linq;
using FluentValidation;
using NetArchTest.Rules;

namespace DAYA.ArchRules.Application.Queries.Queries
{
    class QueryValidatorShouldHaveNameEndingWithQueryValidator : ArchRule
    {
        internal override void Check()
        {
            var queryTypes = Queries.GetTypes();

            var types = Types.InAssembly(Data.ApplicationAssembly)
                .That()
                .Inherit(typeof(AbstractValidator<>))
                .GetTypes()
                .Where(x => queryTypes.Any(y => x.BaseType
                    .GenericTypeArguments
                    .First()
                    .IsAssignableFrom(y)))
                .Where(x => !x.Name.EndsWith("QueryValidator"));

            AssertFailingTypes(types);
        }
    }
}
