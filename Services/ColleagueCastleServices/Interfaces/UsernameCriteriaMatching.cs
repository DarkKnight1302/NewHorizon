using NewHorizon.Models.ColleagueCastleModels;

namespace NewHorizon.Services.ColleagueCastleServices.Interfaces
{
    public class UsernameCriteriaMatching : ICriteriaMatching
    {
        public async Task<bool> IsMatch(PropertyPostDetails propertyPostDetails, SearchPropertyRequest searchPropertyRequest)
        {
            await Task.CompletedTask;

            return searchPropertyRequest.UserId != propertyPostDetails.CreatorUserName;
        }
    }
}
