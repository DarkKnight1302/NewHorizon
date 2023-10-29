using Microsoft.Azure.Cosmos;
using NewHorizon.Extensions;
using NewHorizon.Models.ColleagueCastleModels.DatabaseModels;
using NewHorizon.Repositories.Interfaces;
using NewHorizon.Services.Interfaces;

namespace NewHorizon.Repositories
{
    public class ShotlistedPropertyRepository : IShortListedPropertyRepository
    {
        private readonly Container container;

        public ShotlistedPropertyRepository(ICosmosDbService cosmosDbService)
        {
            this.container = cosmosDbService.GetContainerFromColleagueCastle("ShortlistedProperties");
        }
        public async Task<List<string>> GetShortlistedPropertiesByUser(string userId)
        {
            var shortlistedProperties = await this.container.SafeReadAsync<ShortlistedProperties>(userId, new PartitionKey(userId)).ConfigureAwait(false);
            if (shortlistedProperties == null)
            {
                return null;
            }
            return shortlistedProperties.ShortlistedPropertiesIds.ToList();
        }

        public async Task ShortlistProperty(string postId, string userId)
        {
            var shortlistedProperties = await this.container.SafeReadAsync<ShortlistedProperties>(userId, new PartitionKey(userId)).ConfigureAwait(false);
            if (shortlistedProperties == null)
            {
                shortlistedProperties = new ShortlistedProperties()
                {
                    Id = userId,
                    Uid = userId,
                    UserId = userId
                };
            }
            shortlistedProperties.AddProperty(postId);
            await this.container.UpsertItemAsync(shortlistedProperties, new PartitionKey(userId)).ConfigureAwait(false);
        }
    }
}
