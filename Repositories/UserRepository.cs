using Microsoft.Azure.Cosmos;
using NewHorizon.Models.ColleagueCastleModels.DatabaseModels;
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

        public async Task<bool> CreateAdminUser(string username, string password)
        {
            Models.ColleagueCastleModels.DatabaseModels.User existingUser = await GetUserByUserNameAsync(username).ConfigureAwait(false);
            if (existingUser != null)
            {
                return false;
            }
            (string HashedPassword, string salt) passwordAndSalt = HashingUtil.HashPassword(password);
            Models.ColleagueCastleModels.DatabaseModels.User user = new Models.ColleagueCastleModels.DatabaseModels.User
            {
                Id = username,
                UserName = username,
                Name = "Admin",
                Salt = passwordAndSalt.salt,
                HashedPassword = passwordAndSalt.HashedPassword,
                IsAdmin = true,
                CreatedAt = DateTime.UtcNow
            };
            await this.container.CreateItemAsync(user).ConfigureAwait(false);
            return true;
        }

        public async Task<bool> CreateUserIfNotExist(string username, string password, string name, string phoneNumber, string email, string corporateEmailId)
        {
            username = username.Trim().ToLower();
            Models.ColleagueCastleModels.DatabaseModels.User existingUser = await GetUserByUserNameAsync(username).ConfigureAwait(false);
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
            Models.ColleagueCastleModels.DatabaseModels.User user = new Models.ColleagueCastleModels.DatabaseModels.User
            {
                Id = username,
                UserName = username,
                Name = name,
                PhoneNumber = phoneNumber,
                Email = email.ToLower(),
                Salt = passwordAndSalt.salt,
                HashedPassword = passwordAndSalt.HashedPassword,
                Company = companyName,
                IsAdmin = false,
                CorporateEmailHash = corporateEmailHash,
                CreatedAt = DateTime.UtcNow
            };
            await this.container.CreateItemAsync(user).ConfigureAwait(false);
            return true;
        }

        public async Task<bool> DeleteUser(string username)
        {
            var response = await this.container.DeleteItemAsync<Models.ColleagueCastleModels.DatabaseModels.User>(username, new PartitionKey(username)).ConfigureAwait(false);
            if (response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return true;
            }
            return false;
        }

        public async Task<Models.ColleagueCastleModels.DatabaseModels.User> GetUserByUserNameAsync(string username)
        {
            username = username.ToLower();
            ItemResponse<Models.ColleagueCastleModels.DatabaseModels.User> user = null;
            try
            {
                user = await this.container.ReadItemAsync<Models.ColleagueCastleModels.DatabaseModels.User>(username, new PartitionKey(username)).ConfigureAwait(false);
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

        public async Task<bool> UpdateUserPassword(Models.ColleagueCastleModels.DatabaseModels.User user, string password)
        {
            if (user == null)
            {
                return false;
            }

            (string HashedPassword, string salt) passwordAndSalt = HashingUtil.HashPassword(password);
            user.HashedPassword = passwordAndSalt.HashedPassword;
            user.Salt = passwordAndSalt.salt;
            try
            {
                await this.container.UpsertItemAsync(user).ConfigureAwait(false);
            } catch (CosmosException)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> UserExistForCorporateEmail(string corporateEmail)
        {
            corporateEmail = corporateEmail.Trim().ToLower();
            string corporateEmailHash = HashingUtil.HashEmail(corporateEmail);
            return await UserExistByCorporateEmailHash(corporateEmailHash).ConfigureAwait(false);
        }

        private async Task<bool> UserExistByCorporateEmailHash(string corporateEmailHash)
        {
            QueryDefinition queryDefinition = new QueryDefinition($"SELECT * FROM c WHERE c.CorporateEmailHash = @value")
            .WithParameter("@value", corporateEmailHash);

            // Execute the query and retrieve the results
            FeedIterator<Models.ColleagueCastleModels.DatabaseModels.User> queryResultSet = container.GetItemQueryIterator<Models.ColleagueCastleModels.DatabaseModels.User>(queryDefinition);
            while (queryResultSet.HasMoreResults)
            {
                FeedResponse<Models.ColleagueCastleModels.DatabaseModels.User> currentResultSet = await queryResultSet.ReadNextAsync().ConfigureAwait(false);
                if (currentResultSet != null && currentResultSet.Count > 0)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
