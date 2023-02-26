using NewHorizon.Models.ColleagueCastleModels;
using NewHorizon.Models.ColleagueCastleModels.DatabaseModels;

namespace NewHorizon.Services.ColleagueCastleServices.Interfaces
{
    public interface IPropertyPostService
    {
        public Task<string> CreatePropertyPostAsync(CreatePropertyPostRequest createPropertyPostRequest);

        public Task<bool> UpdatePropertyPostAsync(PropertyPost propertyPost);

        public Task<PropertyPostDetails> GetPropertyPostAsync(string propertyPostId);

        public Task<IEnumerable<PropertyPostDetails>> GetUserPropertyPostsAsync(string userId);

        public Task<bool> DeletePropertyPostAsync(string propertyPostId, string userId);
    }
}
