using System.Threading.Tasks;

using DAYA.Cloud.Framework.V2.Application.InternalCommands;
using DAYA.Cloud.Framework.V2.Common.Constants;
using DAYA.Cloud.Framework.V2.Cosmos.Abstractions;

using Microsoft.Azure.Cosmos;

namespace DAYA.Cloud.Framework.V2.DirectOperations.Repositories
{
    public class InternalMessageRepository : IInternalMessageRepository
    {
        private readonly Container _internalMessageContainer;

        public InternalMessageRepository(IContainerFactory containerFactory)
        {
            _internalMessageContainer = containerFactory.Get(ServiceDatabaseContainersBase.InternalCommands);
        }

        public async Task<InternalCommandMessage> CreateAsync(InternalCommandMessage message)
        {
            var response = await _internalMessageContainer.CreateItemAsync(message, new PartitionKey(message.PartitionKey));
            return response.Resource;
        }

        public async Task DeleteAsync(string id, string partitionKey)
        {
            await _internalMessageContainer.DeleteItemAsync<InternalCommandMessage>(id, new PartitionKey(partitionKey));
        }

        public async Task<InternalCommandMessage> GetByIdAsync(string id, string partitionKey)
        {
            try
            {
                var response = await _internalMessageContainer.ReadItemAsync<InternalCommandMessage>(id, new PartitionKey(partitionKey));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<InternalCommandMessage> UpdateAsync(InternalCommandMessage message)
        {
            var response = await _internalMessageContainer.ReplaceItemAsync(message, message.Id.ToString(), new PartitionKey(message.Id.ToString()));
            return response.Resource;
        }
    }
}