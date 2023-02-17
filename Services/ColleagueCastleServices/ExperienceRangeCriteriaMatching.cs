using NewHorizon.Models.ColleagueCastleModels;
using NewHorizon.Models.ColleagueCastleModels.DatabaseModels;
using NewHorizon.Services.ColleagueCastleServices.Interfaces;

namespace NewHorizon.Services.ColleagueCastleServices
{
    public class ExperienceRangeCriteriaMatching : ICriteriaMatching
    {
        public async Task<bool> IsMatch(PropertyPostDetails propertyPostDetails, SearchPropertyRequest searchPropertyRequest)
        {
            await Task.CompletedTask;

            if (searchPropertyRequest.ExperienceInYears == 0
                || propertyPostDetails.ExperienceRange == null
                || propertyPostDetails.ExperienceRange.MaxExpYears == 0)
            {
                return true;
            }
            
            if (searchPropertyRequest.ExperienceInYears >= propertyPostDetails.ExperienceRange.MinExpYears && searchPropertyRequest.ExperienceInYears <= propertyPostDetails.ExperienceRange.MaxExpYears)
            {
                return true;
            }

            return false;
        }
    }
}
