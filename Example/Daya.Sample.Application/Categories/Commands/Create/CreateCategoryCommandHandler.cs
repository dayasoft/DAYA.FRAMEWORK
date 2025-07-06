using Daya.Sample.Domain.Categories;
using DAYA.Cloud.Framework.V2.DirectOperations.Contracts;
using DAYA.Cloud.Framework.V2.DirectOperations.Repositories;

namespace Daya.Sample.Application.Categories.Commands.Create
{
    public class CreateCategoryCommandHandler : IDirectCommandHandler<CreateCategoryCommand, Guid>
    {
        private readonly ICosmosRepository<Category, CategoryId> _cateogryRepository;

        public CreateCategoryCommandHandler(ICosmosRepository<Category, CategoryId> cateogryRepository)
        {
            _cateogryRepository = cateogryRepository;
        }

        public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await Category.CreateAsync(
                request.CategoryId,
                DefaultValues.PlatformPartitionKey,
                request.Name,
                request.Description);

            await _cateogryRepository.AddAsync(category, cancellationToken);

            return category.Id.Value;
        }
    }
}