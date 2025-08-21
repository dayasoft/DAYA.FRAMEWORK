using System.Diagnostics.CodeAnalysis;
using Daya.Sample.IntegrationTests.SeedWork;
using Daya.Sample.TestHelpers.Application.Categories;
using DAYA.Cloud.Framework.V2.Application.Contracts;
using Xunit;
using Xunit.Abstractions;

namespace Daya.Sample.IntegrationTests.Tests.Categories
{
    [Collection("Database collection")]
    [ExcludeFromCodeCoverage]
    public class CategoryTests
    {
        private readonly IServiceModule _serviceModule;
        private readonly TestFixture _testFixture;

        public CategoryTests(TestFixture fixture, ITestOutputHelper output)
        {
            _testFixture = fixture;
            TestFixture.Output = output;
            _serviceModule = TestFixture.ServiceModule;
        }

        [Fact]
        public async Task Create_Category_Should_Create_Success()
        {
            // Initialize test environment
            await _testFixture.InitializeTestAsync();

            // Arrange
            var categoryId = Guid.NewGuid();
            var createCategoryCommand = new CreateCategoryCommandBuilder()
               .SetCategoryId(categoryId)
               .SetName("Test Category")
               .Build();

            // create user account
            await _serviceModule.ExecuteCommandAsync(createCategoryCommand);

            // get user account by id
            var getCategoriesQuery = new GetCategoriesQueryBuilder()
                .SetPageNumber(1)
                .SetPageSize(10)
                .Build();

            var categoryList = await _serviceModule.ExecuteQueryAsync(getCategoriesQuery);

            // Verify the result
            Assert.NotNull(categoryList);

            // update category
            var updateCategoryCommand = new UpdateCategoryCommandBuilder()
                .SetCategoryId(categoryId)
                .SetName("Updated Category")
                .Build();
            await _serviceModule.ExecuteCommandAsync(updateCategoryCommand);

            // get updated category
            var updatedCategory = await _serviceModule.ExecuteQueryAsync(getCategoriesQuery);

            // Verify the updated category
            Assert.NotNull(updatedCategory);
        }
    }
}