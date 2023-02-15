using NewHorizon.Models.ColleagueCastleModels;

namespace NewHorizon.Services.ColleagueCastleServices.Interfaces
{
    public interface IPropertyMatchingService
    {
        public Task<bool> IsMatch(PropertyPostDetails propertyPostDetails, SearchPropertyRequest searchPropertyRequest);
    }
}
