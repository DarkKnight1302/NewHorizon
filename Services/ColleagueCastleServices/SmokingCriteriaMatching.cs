using NewHorizon.Models.ColleagueCastleModels;
using NewHorizon.Models.ColleagueCastleModels.DatabaseModels;
using NewHorizon.Services.ColleagueCastleServices.Interfaces;

namespace NewHorizon.Services.ColleagueCastleServices
{
    public class SmokingCriteriaMatching : ICriteriaMatching
    {
        public async Task<bool> IsMatch(PropertyPostDetails propertyPostDetails, SearchPropertyRequest searchPropertyRequest)
        {
            await Task.CompletedTask;

            if (searchPropertyRequest.Smoking == Smoking.Ignore || propertyPostDetails.Smoking == Smoking.Allowed)
            {
                return true;
            }

            return propertyPostDetails.Smoking == searchPropertyRequest.Smoking;
        }
    }
}
