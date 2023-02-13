
using NewHorizon.Models.ColleagueCastleModels;

namespace NewHorizon.Repositories.Interfaces
{
    public interface IPropertyPostRepository
    {
        public Task<bool> CreatePropertyPostAsync(string username, string placeId, string title, string description, List<string> images);
    }
}
