using Microsoft.Azure.Cosmos;
using NewHorizon.Models.ColleagueCastleModels.DatabaseModels;
using NewHorizon.Repositories.Interfaces;
using NewHorizon.Services.Interfaces;

namespace NewHorizon.Repositories
{
    public class UserInterestRepository : IUserInterestRepository
    {
        private readonly Container _container;

        public UserInterestRepository(ICosmosDbService cosmosDbService)
        {
            this._container = cosmosDbService.GetContainerFromColleagueCastle("UserInterests");
        }

        public async Task<bool> AddInterestForPostAsync(string postId, string userId)
        {
            UserInterests userInterests = null;
            ItemResponse<UserInterests> itemResponse = null;
            try
            {
                itemResponse = await this._container.ReadItemAsync<UserInterests>(postId, new PartitionKey(postId)).ConfigureAwait(false);
            }
            catch (CosmosException)
            {
                userInterests = null;
            }
            if (itemResponse != null)
            {
                userInterests = itemResponse.Resource;
            }

            if (userInterests == null)
            {
                userInterests = new UserInterests
                {
                    Id = postId,
                    Uid = postId,
                    PostId = postId,
                    InterestedUserIds = new List<string>()
                };
            }
            if (userInterests.InterestedUserIds.Contains(userId))
            {
                // Interest already added.
                return false;
            }
            userInterests.AddUserInterest(userId);
            await this._container.UpsertItemAsync(userInterests).ConfigureAwait(false);
            return true;
        }

        public async Task<List<string>> GetInterestedUsersAsync(string postId)
        {
            ItemResponse<UserInterests> itemResponse = null;
            try
            {
                itemResponse = await this._container.ReadItemAsync<UserInterests>(postId, new PartitionKey(postId)).ConfigureAwait(false);
            }
            catch (CosmosException)
            {
                return null;
            }
            if (itemResponse != null)
            {
                return itemResponse.Resource.InterestedUserIds;
            }
            return null;
        }
    }
}
