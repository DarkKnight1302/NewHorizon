using NewHorizon.Models.ColleagueCastleModels;

namespace NewHorizon.Services.ColleagueCastleServices.Interfaces
{
    public interface IPropertyPostService
    {
        public Task<string> CreatePropertyPostAsync(string sessionId, string placeId, string title, string description, List<string> images);

        public Task<bool> UpdatePropertyPostAsync(PropertyPost propertyPost);

        public Task<PropertyPost> GetPropertyPostAsync(string propertyPostId);

        public Task<bool> DeletePropertyPostAsync(string propertyPostId);
    }
}
