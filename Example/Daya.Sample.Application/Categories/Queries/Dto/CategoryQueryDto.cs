using Daya.Sample.Domain.Categories;

namespace Daya.Sample.Application.Categories.Queries.Dto
{
    public class CategoryQueryDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public List<CategoryTag> Tags { get; set; } = new();
    }
}