using NewHorizon.Helpers;

namespace NewHorizon.Services.Interfaces
{
    public interface ITrafficDataService
    {
        public int FetchAndStoreRouteTimeInMinutes(string fromPlaceId, string toPlaceId, int dayOfWeek, TimeOfDay timeOfDay);
    }
}
