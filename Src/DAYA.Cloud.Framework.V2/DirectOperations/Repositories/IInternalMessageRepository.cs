using System.Threading.Tasks;
using DAYA.Cloud.Framework.V2.Application.InternalCommands;

namespace DAYA.Cloud.Framework.V2.DirectOperations.Repositories;

public interface IInternalMessageRepository
{
    Task<InternalCommandMessage> CreateAsync(InternalCommandMessage message);

    Task<InternalCommandMessage> GetByIdAsync(string id, string partitionKey);

    Task<InternalCommandMessage> UpdateAsync(InternalCommandMessage message);

    Task DeleteAsync(string id, string partitionKey);
}