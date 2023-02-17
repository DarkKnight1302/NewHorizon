using NewHorizon.Models.ColleagueCastleModels;
using NewHorizon.Models.ColleagueCastleModels.DatabaseModels;
using NewHorizon.Services.ColleagueCastleServices.Interfaces;

namespace NewHorizon.Services.ColleagueCastleServices
{
    public class PropertyTypeCriteriaMatching : ICriteriaMatching
    {
        public async Task<bool> IsMatch(PropertyPostDetails propertyPostDetails, SearchPropertyRequest searchPropertyRequest)
        {
            await Task.CompletedTask;

            if (searchPropertyRequest.PropertyType == PropertyType.Ignore)
            {
                return true;
            }
            return propertyPostDetails.PropertyType == searchPropertyRequest.PropertyType;
        }
    }
}
