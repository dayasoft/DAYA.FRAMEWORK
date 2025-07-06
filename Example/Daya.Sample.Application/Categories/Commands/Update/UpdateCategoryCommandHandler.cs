using Daya.Sample.Domain.Categories;
using DAYA.Cloud.Framework.V2.DirectOperations.Contracts;
using DAYA.Cloud.Framework.V2.DirectOperations.Repositories;

namespace Daya.Sample.Application.Categories.Commands.Update
{
    public class UpdateCategoryCommandHandler : IDirectCommandHandler<UpdateCategoryCommand>
    {
        private readonly ICosmosRepository<Category, CategoryId> _cateogryRepository;

        public UpdateCategoryCommandHandler(ICosmosRepository<Category, CategoryId> cateogryRepository)
        {
            _cateogryRepository = cateogryRepository;
        }

        public async Task Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await EnsureCategory(request.CategoryId, cancellationToken);
            category.Update(request.Name);

            await _cateogryRepository.UpdateAsync(category.Id, category, cancellationToken);
        }

        private async Task<Category> EnsureCategory(CategoryId categoryId, CancellationToken cancellationToken)
        {
            var category = await _cateogryRepository.GetByIdAsync(categoryId, DefaultValues.PlatformPartitionKey);
            if (category == null)
            {
                throw new InvalidOperationException($"Category with id {categoryId} not found.");
            }
            return category;
        }
    }
}