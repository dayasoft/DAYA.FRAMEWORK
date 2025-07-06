using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using DAYA.Cloud.Framework.V2.Domain;
using Microsoft.Azure.Cosmos;

namespace DAYA.Cloud.Framework.V2.DirectOperations.Repositories
{
    public interface ICosmosRepository<TAggregateRoot, in TTypedId> : ICosmosRepository
        where TAggregateRoot : CosmosEntity
        where TTypedId : TypedId
    {
        Task<TAggregateRoot?> GetItemAsync(Expression<Func<TAggregateRoot, bool>> predicate, params string[] partitionKeyValues);

        Task<IEnumerable<TAggregateRoot>> GetItemsAsync(Expression<Func<TAggregateRoot, bool>> predicate, CancellationToken cancellationToken = default, params string[] partitionKeyValues);

        Task<TAggregateRoot> GetByIdAsync(TTypedId id, params string[] partitionKeyValues);

        Task<TAggregateRoot?> GetByIdAsync(TTypedId id, CancellationToken cancellationToken = default, params string[] partitionKeyValues);

        Task<IEnumerable<TAggregateRoot>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<IEnumerable<TAggregateRoot>> QueryAsync(string sqlQuery, CancellationToken cancellationToken = default);

        Task AddAsync(TAggregateRoot entity, CancellationToken cancellationToken = default);

        Task UpdateAsync(TTypedId id, TAggregateRoot entity, CancellationToken cancellationToken = default);

        Task DeleteAsync(TTypedId id, params string[] partitionKeyValues);

        Task ReplaceAsync(TTypedId id, TAggregateRoot entity, CancellationToken cancellationToken = default);

        Task<int> GetCountAsync(Expression<Func<TAggregateRoot, bool>> predicate, string partitionKey);
    }

    public interface ICosmosRepository
    {
        Dictionary<PartitionKey, TransactionalBatch> TransactionalBatches { get; }
    }
}