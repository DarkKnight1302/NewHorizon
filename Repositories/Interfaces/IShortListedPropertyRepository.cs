namespace NewHorizon.Repositories.Interfaces
{
    public interface IShortListedPropertyRepository
    {
        public Task ShortlistProperty(string postId, string userId);

        public Task<List<string>> GetShortlistedPropertiesByUser(string userId);
    }
}
