using Daya.Sample.Application.Categories.Queries.Dto;
using DAYA.Cloud.Framework.V2.Application.Contracts;

namespace Daya.Sample.Application.Categories.Queries.GetCategories
{
    public record GetCategoriesQuery(
        int PageNumber,
        int PageSize) : PageableQuery<CategoryQueryDto>(PageNumber, PageSize);
}