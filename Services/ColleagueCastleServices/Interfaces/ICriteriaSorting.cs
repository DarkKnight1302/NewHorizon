using NewHorizon.Models.ColleagueCastleModels.DatabaseModels;
using NewHorizon.Models.ColleagueCastleModels;

namespace NewHorizon.Services.ColleagueCastleServices.Interfaces
{
    public interface ICriteriaSorting
    {
        public Task Sort(List<PropertyPostResponse> propertyPostDetails, SearchPropertyRequest searchPropertyRequest);
    }
}
