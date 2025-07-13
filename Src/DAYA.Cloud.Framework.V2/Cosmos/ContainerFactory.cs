using DAYA.Cloud.Framework.V2.Cosmos.Abstractions;
using Microsoft.Azure.Cosmos;

namespace DAYA.Cloud.Framework.V2.Cosmos;

internal class ContainerFactory : IContainerFactory
{
    private readonly Database _database;

    public ContainerFactory(Database database)
    {
        _database = database;
    }

    public Container Get(string containerName)
    {
        return _database.GetContainer(containerName);
    }
}