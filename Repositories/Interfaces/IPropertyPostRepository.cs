﻿
using NewHorizon.Models.ColleagueCastleModels;
using NewHorizon.Models.ColleagueCastleModels.DatabaseModels;

namespace NewHorizon.Repositories.Interfaces
{
    public interface IPropertyPostRepository
    {
        public Task<string> CreatePropertyPostAsync(CreatePropertyObject createPropertyObject);

        public Task<IEnumerable<PropertyPostDetails>> GetAllPropertyPostDetailsAsync(Location location, string company, int radialDistance);

        public Task<IEnumerable<PropertyPostDetails>> GetAllAvailablePostOfUserAsync(string userId);

        public Task<bool> DeletePropertyPostAsync(PropertyPostDetails propertyPostDetails);

        public Task<PropertyPostDetails> GetPropertyPostDetailsById(string postId);

        public Task<IEnumerable<PropertyPostDetails>> GetPropertyPostDetailsByIds(List<string> postIds);
    }
}
