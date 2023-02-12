using NewHorizon.Models;

namespace NewHorizon.Services.Interfaces
{
    public interface IRouteRegistrationService
    {
        public Task RegisterRoute(string fromPlaceId, string toPlaceID, DateTimeOffset startTime, DateTimeOffset endTime);

        public Task<List<RegisteredRoutes>> GetAllRoutes();

        public Task<IReadOnlyCollection<RegisteredRoutes>> GetAllRoutesForDateTimeAsync(DateTimeOffset dateTime);
    }
}
