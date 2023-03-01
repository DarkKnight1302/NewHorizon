using NewHorizon.Models.ColleagueCastleModels;
using NewHorizon.Models.ColleagueCastleModels.DatabaseModels;

namespace NewHorizon.Services.ColleagueCastleServices.Interfaces
{
    public interface ISearchPropertyService
    {
        public Task<(IReadOnlyList<PropertyPostDetails>?, string error)> GetMatchingPropertyListAsync(SearchPropertyRequest searchPropertyRequest);
    }
}
