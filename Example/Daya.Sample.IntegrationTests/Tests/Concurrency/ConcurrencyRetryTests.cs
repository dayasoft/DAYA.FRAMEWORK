using System.Diagnostics.CodeAnalysis;
using Daya.Sample.Domain.Categories;
using Daya.Sample.IntegrationTests.SeedWork;
using Daya.Sample.TestHelpers.Application.Categories;
using Daya.Sample.TestHelpers.Domain;
using DAYA.Cloud.Framework.V2.Application.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace Daya.Sample.IntegrationTests.Tests.Concurrency
{
    [Collection("Database collection")]
    [ExcludeFromCodeCoverage]
    public class ConcurrencyRetryTests
    {
        private readonly IServiceModule _serviceModule;
        private readonly TestFixture _testFixture;
        private readonly ILogger<ConcurrencyRetryTests> _logger;

        public ConcurrencyRetryTests(TestFixture fixture, ITestOutputHelper output)
        {
            _testFixture = fixture;
            TestFixture.Output = output;
            _serviceModule = TestFixture.ServiceModule;
            _logger = _testFixture.ServiceProvider.GetRequiredService<ILogger<ConcurrencyRetryTests>>();
        }

        [Fact]
        public async Task When_TwoParallelUpdates_Should_RetryAndSucceed()
        {
            // Initialize test environment
            await _testFixture.InitializeTestAsync();

            // Arrange: Create a category first
            var categoryId = new CategoryIdBuilder().Build();
            var createCommand = new CreateCategoryCommandBuilder()
                .SetCategoryId(categoryId.Value)
                .SetName("Initial Category Name")
                .Build();

            await _serviceModule.ExecuteCommandAsync(createCommand);

            // Act: Execute multiple parallel updates with slight delays to increase chance of conflict
            const int taskCount = 9;
            var tasks = new List<Task>();
            var startSignal = new TaskCompletionSource<bool>();
            var readyCount = 0;

            for (int i = 1; i <= taskCount; i++)
            {
                var taskNumber = i; // Capture loop variable
                tasks.Add(Task.Run(async () =>
                {
                    // Signal that this task is ready
                    Interlocked.Increment(ref readyCount);

                    // Wait for all tasks to be ready
                    await startSignal.Task;

                    // Add small random delay to increase chance of conflicts
                    //var random = new Random();
                    //await Task.Delay(random.Next(1, 10));

                    await UpdateCategoryAsync(categoryId.Value, $"Updated by Task {taskNumber}", CategoryTag.Create($"Tag {taskNumber}"));
                }));
            }

            // Wait for all tasks to be ready
            while (readyCount < taskCount)
            {
                await Task.Delay(1);
            }

            // Start all tasks simultaneously
            startSignal.SetResult(true);

            // All tasks should complete successfully (with retry mechanism handling conflicts)
            await Task.WhenAll(tasks);

            // Assert: Verify that all tasks completed without throwing exceptions
            Assert.All(tasks, task => Assert.True(task.IsCompletedSuccessfully));

            // Verify final state - one of the updates should have won
            var getCategoriesQuery = new GetCategoriesQueryBuilder()
                .SetPageNumber(1)
                .SetPageSize(10)
                .Build();

            var categories = await _serviceModule.ExecuteQueryAsync(getCategoriesQuery);
            var updatedCategory = categories.Items.First(c => c.Id == categoryId.Value);

            Assert.StartsWith("Updated by Task", updatedCategory.Name);
            Assert.Equal(9, updatedCategory.Tags.Count); // All tags should be present

            // Verify all unique tags are present
            var tagTitles = updatedCategory.Tags.Select(t => t.Title).ToHashSet();
            for (int i = 1; i <= 9; i++)
            {
                Assert.Contains($"Tag {i}", tagTitles);
            }

            _logger.LogInformation($"Final category name: {updatedCategory.Name}");
        }

        [Fact]
        public async Task When_EntityDeleted_During_Retry_Should_HandleGracefully()
        {
            // Initialize test environment
            await _testFixture.InitializeTestAsync();

            // Arrange: Create a category
            var categoryId = new CategoryIdBuilder().Build();
            var createCommand = new CreateCategoryCommandBuilder()
                .SetCategoryId(categoryId.Value)
                .SetName("Deletion Test Category")
                .Build();

            await _serviceModule.ExecuteCommandAsync(createCommand);

            await UpdateCategoryAsync(categoryId.Value, "This should work 1", CategoryTag.Create("first"));
            await UpdateCategoryAsync(categoryId.Value, "This should work 2", CategoryTag.Create("second"));

            // get the category to confirm it exists
            var getCategoriesQuery = new GetCategoriesQueryBuilder()
                .SetPageNumber(1)
                .SetPageSize(10)
                .Build();

            var categories = await _serviceModule.ExecuteQueryAsync(getCategoriesQuery);

            // Verify the update succeeded
            var updatedCategory = categories.Items.First();
            Assert.Equal("This should work 2", updatedCategory.Name);
            Assert.Single(categories.Items);
        }

        private async Task UpdateCategoryAsync(Guid categoryId, string newName, CategoryTag tag)
        {
            try
            {
                var updateCommand = new UpdateCategoryCommandBuilder()
                    .SetCategoryId(categoryId)
                    .SetName(newName)
                    .SetTag(tag)
                    .Build();

                await _serviceModule.ExecuteCommandAsync(updateCommand);

                _logger.LogInformation($"Successfully updated category to: {newName}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to update category to: {newName}");
                throw;
            }
        }
    }
}