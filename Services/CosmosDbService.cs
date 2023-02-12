using Microsoft.Azure.Cosmos;
using NewHorizon.Controllers;
using NewHorizon.Services.Interfaces;
using System.Collections.Concurrent;

namespace NewHorizon.Services
{
    public class CosmosDbService : ICosmosDbService
    {
        private readonly CosmosClient cosmosClient;
        private readonly ConcurrentDictionary<string, Container> containerCache = new ConcurrentDictionary<string, Container>();

        public CosmosDbService(ILogger<TrafficDurationController> logger, ISecretService secretService)
        {
            string connectionString = secretService.GetSecretValue("CONNECTION_STRING_NEW_HORIZONDATABASE");
            this.cosmosClient = new CosmosClient(connectionString);
        }

        public Container GetContainer(string databaseName, string containerName)
        {
            string key = databaseName + "_" + containerName;
            return this.containerCache.GetOrAdd(key, (k) =>
            {
                Database database = cosmosClient.GetDatabase(databaseName);
                return database.GetContainer(containerName);
            });
        }

        public Container GetContainerFromColleagueCastle(string containerName)
        {
            return this.GetContainer("ColleagueCastle", containerName);
        }
    }
}
