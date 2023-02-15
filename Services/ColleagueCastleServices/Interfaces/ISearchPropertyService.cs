using NewHorizon.Models.ColleagueCastleModels;

namespace NewHorizon.Services.ColleagueCastleServices.Interfaces
{
    public interface ISearchPropertyService
    {
        public Task<IReadOnlyList<PropertyPostDetails>> GetMatchingPropertyListAsync(SearchPropertyRequest searchPropertyRequest);
    }
}
