using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DAYA.Cloud.Framework.V2.Cosmos.Abstractions;
using DAYA.Cloud.Framework.V2.DirectOperations.Attributes;
using DAYA.Cloud.Framework.V2.DirectOperations.Helpers;
using DAYA.Cloud.Framework.V2.Domain;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace DAYA.Cloud.Framework.V2.DirectOperations.Repositories
{
    public class CosmosRepository<TAggregateRoot, TTypedId> : ICosmosRepository<TAggregateRoot, TTypedId>
        where TAggregateRoot : CosmosEntity
        where TTypedId : TypedId
    {
        private readonly Container _container;
        private readonly string _containerName;

        private readonly ICosmosEntityChangeTracker _cosmosEntityChangeTracker;
        public Dictionary<PartitionKey, TransactionalBatch> TransactionalBatches { get; } = new();

        public CosmosRepository(IContainerFactory containerFactory, ICosmosEntityChangeTracker cosmosEntityChangeTracker)
        {
            _cosmosEntityChangeTracker = cosmosEntityChangeTracker;
            _containerName = typeof(TAggregateRoot).GetCustomAttribute<ContainerNameAttribute>()?.ContainerName;

            if (string.IsNullOrEmpty(_containerName))
            {
                throw new InvalidOperationException($"The '{nameof(ContainerNameAttribute)}' not found for {typeof(TAggregateRoot).Name} or '{nameof(ContainerNameAttribute.ContainerName)}' is null or empty");
            }

            _container = containerFactory.Get(_containerName);
        }

        public async Task<TAggregateRoot?> GetItemAsync(Expression<Func<TAggregateRoot, bool>> predicate, params string[] partitionKeyValues)
        {
            var requestOptions = new QueryRequestOptions
            {
                PartitionKey = PartitionKeyHelper.GetPartitionKey(partitionKeyValues),
                MaxItemCount = 1
            };

            var linqOption = new CosmosLinqSerializerOptions
            {
                PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
            };

            var iterator = _container.GetItemLinqQueryable<TAggregateRoot>(true, null, requestOptions, linqOption)
                        .Where(predicate)
                        .Take(1)
                        .ToFeedIterator();

            while (iterator.HasMoreResults)
            {
                var next = await iterator.ReadNextAsync();
                return next.FirstOrDefault();
            }

            return default;
        }

        public async Task<IEnumerable<TAggregateRoot>> GetItemsAsync(Expression<Func<TAggregateRoot, bool>> predicate, CancellationToken cancellationToken = default, params string[] partitionKeyValues)
        {
            List<TAggregateRoot> results = new List<TAggregateRoot>();

            var requestOptions = new QueryRequestOptions
            {
                PartitionKey = PartitionKeyHelper.GetPartitionKey(partitionKeyValues)
            };

            var linqOption = new CosmosLinqSerializerOptions
            {
                PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
            };

            var iterator = _container.GetItemLinqQueryable<TAggregateRoot>(requestOptions: requestOptions, linqSerializerOptions: linqOption)
                        .Where(predicate)
                        .ToFeedIterator();

            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync(cancellationToken);
                results.AddRange(response.ToList());
            }

            return results;
        }

        public virtual async Task<TAggregateRoot> GetByIdAsync(TTypedId id, params string[] partitionKeyValues)
        {
            try
            {
                ItemResponse<TAggregateRoot> response = await _container.ReadItemAsync<TAggregateRoot>(
                    id.ToString(),
                    PartitionKeyHelper.GetPartitionKey(partitionKeyValues));

                if (response.Resource is not null)
                {
                    response.Resource.ETag = response.ETag;
                    Track(response.Resource);
                }

                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return default;
            }
        }

        public async Task<int> GetCountAsync(Expression<Func<TAggregateRoot, bool>> predicate, string partitionKey)
        {
            var requestOptions = new QueryRequestOptions
            {
                PartitionKey = new PartitionKey(partitionKey)
            };

            var linqOption = new CosmosLinqSerializerOptions
            {
                PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
            };

            var queryable = _container.GetItemLinqQueryable<TAggregateRoot>(allowSynchronousQueryExecution: true, requestOptions: requestOptions, linqSerializerOptions: linqOption);

            // Query to get all records
            var countQuery = await queryable
                .Where(predicate) // Add any filter or condition if needed
                .CountAsync(); // Get the count of records

            return countQuery;
        }

        public virtual async Task<TAggregateRoot?> GetByIdAsync(TTypedId id, CancellationToken cancellationToken = default, params string[] partitionKeyValues)
        {
            try
            {
                ItemResponse<TAggregateRoot> response = await _container.ReadItemAsync<TAggregateRoot>(
                    id.ToString(),
                    PartitionKeyHelper.GetPartitionKey(partitionKeyValues),
                    cancellationToken: cancellationToken);

                if (response.Resource is not null)
                {
                    response.Resource.ETag = response.ETag;
                    Track(response.Resource);
                }

                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return default;
            }
        }

        public virtual async Task<IEnumerable<TAggregateRoot>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var query = _container.GetItemQueryIterator<TAggregateRoot>(new QueryDefinition("SELECT * FROM c"));
            List<TAggregateRoot> results = new List<TAggregateRoot>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync(cancellationToken: cancellationToken);
                results.AddRange(response.ToList());
            }

            results.ForEach(Track);

            return results;
        }

        [Obsolete("Use QueryAsync with parameters to prevent SQL injection. This method will be removed in a future version.")]
        public virtual async Task<IEnumerable<TAggregateRoot>> QueryAsync(string sqlQuery, CancellationToken cancellationToken = default)
        {
            // Log warning about potential security risk
            Console.WriteLine("WARNING: QueryAsync(string) is deprecated due to SQL injection risk. Use QueryAsync(QueryDefinition) instead.");

            var query = _container.GetItemQueryIterator<TAggregateRoot>(new QueryDefinition(sqlQuery));

            List<TAggregateRoot> results = new List<TAggregateRoot>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync(cancellationToken: cancellationToken);
                results.AddRange(response.ToList());
            }

            results.ForEach(Track);

            return results;
        }

        public virtual async Task<IEnumerable<TAggregateRoot>> QueryAsync(QueryDefinition queryDefinition, CancellationToken cancellationToken = default)
        {
            if (queryDefinition == null)
                throw new ArgumentNullException(nameof(queryDefinition));

            var query = _container.GetItemQueryIterator<TAggregateRoot>(queryDefinition);

            List<TAggregateRoot> results = new List<TAggregateRoot>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync(cancellationToken: cancellationToken);
                results.AddRange(response.ToList());
            }

            results.ForEach(Track);

            return results;
        }

        public virtual async Task AddAsync(TAggregateRoot entity, CancellationToken cancellationToken = default)
        {
            var transaction = GetOrCreateTransactionalBatch(PartitionKeyHelper.GetPartitionKey(entity));
            transaction.CreateItem(entity);
            Track(entity);
        }

        public virtual async Task UpdateAsync(TTypedId id, TAggregateRoot entity, CancellationToken cancellationToken = default)
        {
            var transaction = GetOrCreateTransactionalBatch(PartitionKeyHelper.GetPartitionKey(entity));
            
            if (!string.IsNullOrEmpty(entity.ETag))
            {
                var itemRequestOptions = new TransactionalBatchItemRequestOptions
                {
                    IfMatchEtag = entity.ETag
                };
                transaction.UpsertItem(entity, itemRequestOptions);
            }
            else
            {
                transaction.UpsertItem(entity);
            }
            
            Track(entity);
        }

        public virtual async Task DeleteAsync(TTypedId id, params string[] partitionKeyValues)
        {
            var entity = await GetByIdAsync(id, partitionKeyValues);
            if (entity is not null)
            {
                var transaction = GetOrCreateTransactionalBatch(PartitionKeyHelper.GetPartitionKey(partitionKeyValues));
                transaction.DeleteItem(id.ToString());
            }
        }

        public virtual async Task ReplaceAsync(TTypedId id, TAggregateRoot entity, CancellationToken cancellationToken = default)
        {
            var transaction = GetOrCreateTransactionalBatch(PartitionKeyHelper.GetPartitionKey(entity));
            
            if (!string.IsNullOrEmpty(entity.ETag))
            {
                var itemRequestOptions = new TransactionalBatchItemRequestOptions
                {
                    IfMatchEtag = entity.ETag
                };
                transaction.ReplaceItem(id.ToString(), entity, itemRequestOptions);
            }
            else
            {
                transaction.ReplaceItem(id.ToString(), entity);
            }
            
            Track(entity);
        }

        public virtual TransactionalBatch GetOrCreateTransactionalBatch(PartitionKey partitionKey)
        {
            if (TransactionalBatches.TryGetValue(partitionKey, out var transactionalBatch))
            {
                return transactionalBatch;
            }

            transactionalBatch = _container.CreateTransactionalBatch(partitionKey);
            if (TransactionalBatches.TryAdd(partitionKey, transactionalBatch))
            {
                return transactionalBatch;
            }

            throw new Exception("Can not create transactional batch.");
        }

        private void Track(TAggregateRoot entity)
        {
            _cosmosEntityChangeTracker.Track(entity, _containerName);
        }
    }
}