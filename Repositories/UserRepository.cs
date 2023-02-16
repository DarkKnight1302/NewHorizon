using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;
using NewHorizon.Models.ColleagueCastleModels;
using NewHorizon.Repositories.Interfaces;
using NewHorizon.Services.Interfaces;
using NewHorizon.Utils;

namespace NewHorizon.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly Container container;

        public UserRepository(ICosmosDbService cosmosDbService)
        {
            this.container = cosmosDbService.GetContainerFromColleagueCastle("User");
        }

        public async Task<bool> CreateUserIfNotExist(string username, string password, string name, string phoneNumber, string email, string corporateEmailId)
        {
            username = username.Trim();
            Models.ColleagueCastleModels.User existingUser = await GetUserByUserNameAsync(username).ConfigureAwait(false);
            if (existingUser != null)
            {
                return false;
            }
            corporateEmailId = corporateEmailId.Trim().ToLower();
            string corporateEmailHash = HashingUtil.HashEmail(corporateEmailId);
            bool userExist = await this.UserExistByCorporateEmailHash(corporateEmailHash).ConfigureAwait(false);
            if (userExist)
            {
                return false;
            }

            (string HashedPassword, string salt) passwordAndSalt = HashingUtil.HashPassword(password);
            string companyName = corporateEmailId.Split('@')[1].ToLower();
            Models.ColleagueCastleModels.User user = new Models.ColleagueCastleModels.User
            {
                Id = username,
                UserName = username,
                Name = name,
                PhoneNumber = phoneNumber,
                Email = email,
                Salt = passwordAndSalt.salt,
                HashedPassword = passwordAndSalt.HashedPassword,
                Company = companyName,
                CorporateEmailHash = corporateEmailHash
            };
            await this.container.CreateItemAsync(user).ConfigureAwait(false);
            return true;
        }

        public async Task<bool> DeleteUser(string username)
        {
            var response = await this.container.DeleteItemAsync<Models.ColleagueCastleModels.User>(username, new PartitionKey(username)).ConfigureAwait(false);
            if (response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return true;
            }
            return false;
        }

        public async Task<Models.ColleagueCastleModels.User> GetUserByUserNameAsync(string username)
        {
            ItemResponse<Models.ColleagueCastleModels.User> user = null;
            try
            {
                user = await this.container.ReadItemAsync<Models.ColleagueCastleModels.User>(username, new PartitionKey(username)).ConfigureAwait(false);
            }
            catch (CosmosException)
            {
                user = null;
            }
            if (user != null)
            {
                return user.Resource;
            }
            return null;
        }

        private async Task<bool> UserExistByCorporateEmailHash(string corporateEmailHash)
        {
            QueryDefinition queryDefinition = new QueryDefinition($"SELECT * FROM c WHERE c.CorporateEmailHash = @value")
            .WithParameter("@value", corporateEmailHash);

            // Execute the query and retrieve the results
            FeedIterator<Models.ColleagueCastleModels.User> queryResultSet = container.GetItemQueryIterator<Models.ColleagueCastleModels.User>(queryDefinition);
            while (queryResultSet.HasMoreResults)
            {
                FeedResponse<Models.ColleagueCastleModels.User> currentResultSet = await queryResultSet.ReadNextAsync().ConfigureAwait(false);
                if (currentResultSet != null && currentResultSet.Count > 0)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
