using Microsoft.Azure.Cosmos;
using NewHorizon.Models.ColleagueCastleModels;
using NewHorizon.Repositories.Interfaces;
using NewHorizon.Services.Interfaces;

namespace NewHorizon.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly Container container;

        public UserRepository(ICosmosDbService cosmosDbService) 
        {
            this.container = cosmosDbService.GetContainerFromColleagueCastle("User");
        }
        public async Task<Models.ColleagueCastleModels.User> GetUserByUserNameAsync(string username)
        {
            ItemResponse<Models.ColleagueCastleModels.User> user = null;
            try
            {
                user = await this.container.ReadItemAsync<Models.ColleagueCastleModels.User>(username, new PartitionKey(username)).ConfigureAwait(false);
            } 
            catch(CosmosException)
            {
                user = null;
            }
            if (user != null)
            {
                return user.Resource;
            }
            return null;
        }
    }
}
