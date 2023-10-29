using Microsoft.Azure.Cosmos;

namespace NewHorizon.Extensions
{
    public static class CosmosContainerExtension
    {
        public static async Task<T> SafeReadAsync<T>(this Container container, string id, PartitionKey partitionKey)
        {
            try
            {
                return await container.ReadItemAsync<T>(id, partitionKey).ConfigureAwait(false);
            }
            catch (CosmosException)
            {
                return default(T);
            }
        }
    }
}
