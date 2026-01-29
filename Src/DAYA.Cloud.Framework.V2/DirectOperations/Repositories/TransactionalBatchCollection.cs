using System.Collections.Generic;
using Microsoft.Azure.Cosmos;

namespace DAYA.Cloud.Framework.V2.DirectOperations.Repositories;

public class TransactionalBatchCollection
{
    private const int MaxOperationsPerBatch = 100;

    private readonly Container _container;
    private readonly PartitionKey _partitionKey;

    private readonly List<TransactionalBatch> _batches = new();
    private int _currentBatchOperationCount;

    public TransactionalBatchCollection(Container container, PartitionKey partitionKey)
    {
        _container = container;
        _partitionKey = partitionKey;

        _batches.Add(_container.CreateTransactionalBatch(_partitionKey));
        _currentBatchOperationCount = 0;
    }

    /// <summary>
    /// Returns the current TransactionalBatch, rolling over to a new one
    /// if the current batch has reached the operation limit.
    /// The caller must add exactly one operation after calling this method.
    /// </summary>
    public TransactionalBatch GetCurrentBatch()
    {
        if (_currentBatchOperationCount >= MaxOperationsPerBatch)
        {
            _batches.Add(_container.CreateTransactionalBatch(_partitionKey));
            _currentBatchOperationCount = 0;
        }

        _currentBatchOperationCount++;
        return _batches[^1];
    }

    /// <summary>
    /// Returns all accumulated batches for execution.
    /// </summary>
    public IReadOnlyList<TransactionalBatch> Batches => _batches.AsReadOnly();
}
