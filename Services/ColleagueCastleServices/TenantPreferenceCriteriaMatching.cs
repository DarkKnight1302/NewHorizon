using NewHorizon.Models.ColleagueCastleModels;
using NewHorizon.Models.ColleagueCastleModels.DatabaseModels;
using NewHorizon.Services.ColleagueCastleServices.Interfaces;

namespace NewHorizon.Services.ColleagueCastleServices
{
    public class TenantPreferenceCriteriaMatching : ICriteriaMatching
    {
        public async Task<bool> IsMatch(PropertyPostDetails propertyPostDetails, SearchPropertyRequest searchPropertyRequest)
        {
            await Task.CompletedTask;

            if (searchPropertyRequest.TenantPreference == TenantPreference.Ignore || propertyPostDetails.TenantPreference == TenantPreference.NoPreference)
            {
                return true;
            }

            return propertyPostDetails.TenantPreference == searchPropertyRequest.TenantPreference;
        }
    }
}
