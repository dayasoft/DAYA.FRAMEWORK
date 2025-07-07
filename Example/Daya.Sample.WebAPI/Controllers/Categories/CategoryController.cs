using Daya.Sample.Application.Categories.Commands.Create;
using Daya.Sample.Application.Categories.Queries.Dto;
using Daya.Sample.Application.Categories.Queries.GetCategories;
using Daya.Sample.WebAPI.Controllers.Categories.Requests;
using DAYA.Cloud.Framework.V2.Application.Contracts;
using DAYA.Cloud.Framework.V2.Authentication.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Daya.Sample.WebAPI.Controllers.Categories
{
    [ApiController]
    [Route("[controller]")]
    [ApiExplorerSettings(GroupName = SwaggerUiExtension.PublicApiGroup)]
    public class CategoryController : ControllerBase
    {
        private readonly IServiceModule _serviceModule;

        public CategoryController(IServiceModule serviceModule)
        {
            _serviceModule = serviceModule;
        }

        /// <summary>
        /// Creates a new category.
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Category Id</returns>
        [HttpPost]
        [DayaAuthorize(DayaPolicyNames.EntraExternalIdOnly)]
        [ProducesResponseType(typeof(Guid), statusCode: 200)]
        public async Task<IActionResult> CreateCategory(
            [FromBody] CreateCategoryRequest request)
        {
            var categoryId = await _serviceModule.ExecuteCommandAsync(
                    new CreateCategoryCommand(
                        new CategoryId(Guid.NewGuid()),
                        request.Name,
                        request.Description)
                );

            return Ok(categoryId);
        }

        /// <summary>
        /// Retrieves a paginated list of categories.
        /// </summary>
        /// <param name="pageNumber">page number</param>
        /// <param name="pageSize">page size</param>
        /// <returns>Category List</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedDto<CategoryQueryDto>), statusCode: 200)]
        public async Task<IActionResult> GetCategories(
            [FromQuery] int pageNumber,
            [FromQuery] int pageSize)
        {
            var queryDto = await _serviceModule.ExecuteQueryAsync(
                new GetCategoriesQuery(
                    pageNumber,
                    pageSize
                ));

            return Ok(queryDto);
        }
    }
}