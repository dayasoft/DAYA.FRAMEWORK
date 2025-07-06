using System.Threading.Tasks;
using DAYA.Cloud.Framework.V2.Application.Outbox;

namespace DAYA.Cloud.Framework.V2.DirectOperations.Repositories;

public interface IOutboxMessageRepository
{
    Task<OutboxMessage> CreateAsync(OutboxMessage message);

    Task<OutboxMessage> GetByIdAsync(string id, string partitionKey);

    Task<OutboxMessage> UpdateAsync(OutboxMessage message);

    Task DeleteAsync(string id, string partitionKey);
}