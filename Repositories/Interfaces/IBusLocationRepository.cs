using NewHorizon.Models;

namespace NewHorizon.Repositories.Interfaces
{
    public interface IBusLocationRepository
    {
        public Task SaveBusCoordindates(double lat, double lng, string busId);

        public Task<BusLocation> GetBusCoordinates(string busId);
    }
}
