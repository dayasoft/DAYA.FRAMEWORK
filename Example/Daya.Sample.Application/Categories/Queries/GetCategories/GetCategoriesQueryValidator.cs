using FluentValidation;

namespace Daya.Sample.Application.Categories.Queries.GetCategories
{
    public class GetCategoriesQueryValidator : AbstractValidator<GetCategoriesQuery>
    {
        public GetCategoriesQueryValidator()
        {
            RuleFor(x => x.PageNumber).GreaterThan(0)
                .WithMessage("PageNumber must be greater than 0");

            RuleFor(x => x.PageSize).GreaterThan(0)
                .WithMessage("PageSize must be greater than 0");
        }
    }
}