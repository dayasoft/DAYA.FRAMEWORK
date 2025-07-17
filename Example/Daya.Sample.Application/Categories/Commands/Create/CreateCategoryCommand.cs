using Daya.Sample.Domain.Categories;
using DAYA.Cloud.Framework.V2.DirectOperations;

namespace Daya.Sample.Application.Categories.Commands.Create
{
    public record CreateCategoryCommand(
        CategoryId CategoryId,
        string Name,
        string Description) : DirectCommand<Guid>;
}