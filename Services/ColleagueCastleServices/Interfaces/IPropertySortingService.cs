using NewHorizon.Models.ColleagueCastleModels;
using NewHorizon.Models.ColleagueCastleModels.DatabaseModels;

namespace NewHorizon.Services.ColleagueCastleServices.Interfaces
{
    public interface IPropertySortingService
    {
        public Task<List<PropertyPostResponse>> SortProperties(IEnumerable<PropertyPostResponse> propertyPostDetails, SearchPropertyRequest searchPropertyRequest);
    }
}
