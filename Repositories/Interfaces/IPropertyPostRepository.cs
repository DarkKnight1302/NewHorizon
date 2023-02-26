
using NewHorizon.Models.ColleagueCastleModels;
using NewHorizon.Models.ColleagueCastleModels.DatabaseModels;

namespace NewHorizon.Repositories.Interfaces
{
    public interface IPropertyPostRepository
    {
        public Task<string> CreatePropertyPostAsync(CreatePropertyObject createPropertyObject);

        public Task<IEnumerable<PropertyPostDetails>> GetAllPropertyPostDetailsAsync(string city, string company);

        public Task<IEnumerable<PropertyPostDetails>> GetAllAvailablePostOfUserAsync(string userId);

        public Task<bool> DeletePropertyPostAsync(PropertyPostDetails propertyPostDetails);

        public Task<PropertyPostDetails> GetPropertryPostDetailsById(string postId);
    }
}
