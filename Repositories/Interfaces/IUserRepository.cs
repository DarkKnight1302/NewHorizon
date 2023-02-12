using NewHorizon.Models.ColleagueCastleModels;

namespace NewHorizon.Repositories.Interfaces
{
    public interface IUserRepository
    {
        public Task<User> GetUserByUserNameAsync(string username);
    }
}
