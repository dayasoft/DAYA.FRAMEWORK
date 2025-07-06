using Microsoft.Azure.Cosmos;

namespace DAYA.Cloud.Framework.V2.Cosmos.Abstractions;

public interface IContainerFactory
{
    Container Get(string containerName);
}
