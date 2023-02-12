using Microsoft.Azure.Cosmos;

namespace NewHorizon.Services.Interfaces
{
    public interface ICosmosDbService
    {
        public Container GetContainer(string databaseName, string containerName);
    }
}
