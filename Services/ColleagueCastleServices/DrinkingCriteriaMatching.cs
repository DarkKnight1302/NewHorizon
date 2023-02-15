using NewHorizon.Models.ColleagueCastleModels;
using NewHorizon.Services.ColleagueCastleServices.Interfaces;

namespace NewHorizon.Services.ColleagueCastleServices
{
    public class DrinkingCriteriaMatching : ICriteriaMatching
    {
        public async Task<bool> IsMatch(PropertyPostDetails propertyPostDetails, SearchPropertyRequest searchPropertyRequest)
        {
            await Task.CompletedTask;

            if (searchPropertyRequest.Drinking == Drinking.Ignore || propertyPostDetails.Drinking == Drinking.Allowed)
            {
                return true;
            }

            return propertyPostDetails.Drinking == searchPropertyRequest.Drinking;
        }
    }
}
