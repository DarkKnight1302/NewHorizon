using NewHorizon.Models.ColleagueCastleModels;
using NewHorizon.Models.ColleagueCastleModels.DatabaseModels;
using NewHorizon.Services.ColleagueCastleServices.Interfaces;

namespace NewHorizon.Services.ColleagueCastleServices
{
    public class FurnishingTypeCriteriaMatching : ICriteriaMatching
    {
        public async Task<bool> IsMatch(PropertyPostDetails propertyPostDetails, SearchPropertyRequest searchPropertyRequest)
        {
            await Task.CompletedTask;

            if (searchPropertyRequest.FurnishingType == FurnishingType.Ignore)
            {
                return true;
            }

            return propertyPostDetails.FurnishingType == searchPropertyRequest.FurnishingType;
        }
    }
}
