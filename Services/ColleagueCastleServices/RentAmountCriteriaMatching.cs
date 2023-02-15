using NewHorizon.Models.ColleagueCastleModels;
using NewHorizon.Services.ColleagueCastleServices.Interfaces;

namespace NewHorizon.Services.ColleagueCastleServices
{
    public class RentAmountCriteriaMatching : ICriteriaMatching
    {
        public async Task<bool> IsMatch(PropertyPostDetails propertyPostDetails, SearchPropertyRequest searchPropertyRequest)
        {
            await Task.CompletedTask;

            if (searchPropertyRequest.MaximumRent == 0 || propertyPostDetails.RentAmount == 0)
            {
                return true;
            }

            int rentAsk = propertyPostDetails.RentAmount;

            if (rentAsk > searchPropertyRequest.MaximumRent)
            {
                return false;
            }

            return true;
        }
    }
}
