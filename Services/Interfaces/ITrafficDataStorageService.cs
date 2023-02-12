using NewHorizon.Helpers;

namespace NewHorizon.Services.Interfaces
{
    public interface ITrafficDataStorageService
    {
        public void StoreRouteData(string fromPlaceId, string toPlaceId, int dayOfWeek, TimeOfDay timeOfDay, int timeInMins);

        public Task<Dictionary<string, int>> GetMedianRouteTimeAsync(string fromPlaceId, string toPlaceId, int dayofWeek);
    }
}
