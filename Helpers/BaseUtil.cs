using NewHorizon.Models.ColleagueCastleModels;
using NewHorizon.Models.ColleagueCastleModels.DatabaseModels;

namespace NewHorizon.Helpers
{
    public static class BaseUtil
    {
        public static IEnumerable<PropertyPostResponse> ConvertPropertyDetails(IEnumerable<PropertyPostDetails> propertyDetails)
        {
            if (propertyDetails == null)
            {
                return null;
            }
            return propertyDetails.Select<PropertyPostDetails, PropertyPostResponse>(x => new PropertyPostResponse { Id = x.Id, CreatorUserName = x.CreatorUserName, Description = x.Description, Drinking = x.Drinking, ExperienceRange = x.ExperienceRange, FlatType = x.FlatType, FoodPreference = x.FoodPreference, FormattedAddress = x.FormattedAddress, FurnishingType = x.FurnishingType, Images = x.Images, InterestIds = x.InterestIds, MapUrl = x.MapUrl, PropertyType = x.PropertyType, RentAmount = x.RentAmount, Smoking = x.Smoking, TenantPreference = x.TenantPreference, Title = x.Title, Views = x.Views, Location = x.Location });
        }
    }
}
