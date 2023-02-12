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
            Models.ColleagueCastleModels.User existingUser = await GetUserByUserNameAsync(username).ConfigureAwait(false);
            if (existingUser != null)
            {
                return false;
            }
            (string HashedPassword, string salt) passwordAndSalt = PasswordHashingUtil.HashPassword(password);
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
            };
            await this.container.CreateItemAsync(user).ConfigureAwait(false);
            return true;
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
    }
}
