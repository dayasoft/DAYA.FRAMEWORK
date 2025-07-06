using FluentValidation;

namespace Daya.Sample.Application.Categories.Commands.Create
{
    public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
    {
        public CreateCategoryCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required.")
                .MinimumLength(2)
                .WithErrorCode("3331");
        }
    }
}