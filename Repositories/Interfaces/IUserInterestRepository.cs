namespace NewHorizon.Repositories.Interfaces
{
    public interface IUserInterestRepository
    {
        public  Task<List<string>> GetInterestedUsersAsync(string postId);

        public Task<bool> AddInterestForPostAsync(string postId, string userId);

        public Task DeleteUserInterestAsync(string postId);

    }
}
