using NewHorizon.Models.ColleagueCastleModels.DatabaseModels;

namespace NewHorizon.Repositories.Interfaces
{
    public interface IUserRepository
    {
        public Task<User> GetUserByUserNameAsync(string username);

        public Task<bool> CreateUserIfNotExist(string username, string password, string name, string phoneNumber, string email, string corporateEmailId);

        public Task<bool> CreateAdminUser(string username, string password);

        public Task<bool> DeleteUser(string username);

        public Task<bool> UpdateUserPassword(User user, string password);

        public Task<bool> UserExistForCorporateEmail(string corporateEmail);
    }
}
