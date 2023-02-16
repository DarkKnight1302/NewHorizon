using NewHorizon.Models.ColleagueCastleModels;

namespace NewHorizon.Repositories.Interfaces
{
    public interface IUserRepository
    {
        public Task<User> GetUserByUserNameAsync(string username);

        public Task<bool> CreateUserIfNotExist(string username, string password, string name, string phoneNumber, string email, string corporateEmailId);

        public Task<bool> DeleteUser(string username);
    }
}
