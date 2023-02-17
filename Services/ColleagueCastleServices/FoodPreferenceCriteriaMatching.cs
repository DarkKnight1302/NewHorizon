using NewHorizon.Models.ColleagueCastleModels;
using NewHorizon.Models.ColleagueCastleModels.DatabaseModels;
using NewHorizon.Services.ColleagueCastleServices.Interfaces;

namespace NewHorizon.Services.ColleagueCastleServices
{
    public class FoodPreferenceCriteriaMatching : ICriteriaMatching
    {
        public async Task<bool> IsMatch(PropertyPostDetails propertyPostDetails, SearchPropertyRequest searchPropertyRequest)
        {
            await Task.CompletedTask;

            if (searchPropertyRequest.FoodPreference == FoodPreference.Ignore || propertyPostDetails.FoodPreference == FoodPreference.None)
            {
                return true;
            }

            return propertyPostDetails.FoodPreference == searchPropertyRequest.FoodPreference;
        }
    }
}
