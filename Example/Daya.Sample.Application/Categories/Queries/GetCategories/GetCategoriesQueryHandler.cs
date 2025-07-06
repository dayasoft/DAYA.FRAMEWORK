using Daya.Sample.Application.Categories.Queries.Dto;
using DAYA.Cloud.Framework.V2.Application.Configuration.Queries;
using DAYA.Cloud.Framework.V2.Application.Contracts;
using DAYA.Cloud.Framework.V2.Cosmos.Abstractions;
using Microsoft.Azure.Cosmos;

namespace Daya.Sample.Application.Categories.Queries.GetCategories
{
    internal class GetCategoriesQueryHandler : IPageableQueryHandler<GetCategoriesQuery, CategoryQueryDto>
    {
        private readonly Container _container;

        public GetCategoriesQueryHandler(IContainerFactory factory)
        {
            _container = factory.Get(ServiceDatabaseContainers.Categories);
        }

        public async Task<PagedDto<CategoryQueryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            var fromWhere = @$"FROM	categories";

            var sql = @$"SELECT * {fromWhere} ";

            var query = new QueryDefinition(sql);

            var publishedJobs = await _container.AsPagedAsync(request, query, fromWhere);

            return publishedJobs;
        }
    }
}