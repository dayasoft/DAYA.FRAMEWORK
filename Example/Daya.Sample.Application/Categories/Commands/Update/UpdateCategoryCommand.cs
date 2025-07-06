using DAYA.Cloud.Framework.V2.DirectOperations;

namespace Daya.Sample.Application.Categories.Commands.Update
{
    public record UpdateCategoryCommand(
        CategoryId CategoryId,
        string Name,
        string Description) : DirectCommand;
}