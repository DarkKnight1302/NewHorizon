using Microsoft.Azure.Cosmos;
using NewHorizon.Helpers;
using NewHorizon.Models;
using NewHorizon.Models.ColleagueCastleModels.DatabaseModels;
using NewHorizon.Repositories.Interfaces;
using NewHorizon.Services.Interfaces;

namespace NewHorizon.Repositories
{
    public class BusLocationRepository : IBusLocationRepository
    {
        private readonly Container container;
        private RequestThresholdPerDay requestThresholdPerDay;

        public BusLocationRepository(ICosmosDbService cosmosDbService)
        {
            this.container = cosmosDbService.GetContainer("BusLocator", "BusLocation");
            this.requestThresholdPerDay = new RequestThresholdPerDay(4000);
        }

        public async Task<BusLocation> GetBusCoordinates(string busId)
        {
            try
            {
                BusLocationDbModel dbResponse = await this.container.ReadItemAsync<BusLocationDbModel>(busId, new PartitionKey(busId));
                if (dbResponse == null)
                {
                    return null;
                }
                return dbResponse.BusLocation;
            }
            catch (CosmosException)
            {
                return null;
            }
        }

        public async Task SaveBusCoordindates(double lat, double lng, string busId)
        {
            if (this.requestThresholdPerDay.AllowRequest())
            {
                var busLocation = new BusLocation { Latitude = lat, Longitude = lng };
                var busDbModel = new BusLocationDbModel
                {
                    Id = busId,
                    BusId = busId,
                    BusLocation = busLocation,
                };
                await this.container.UpsertItemAsync(busDbModel);
            }
        }
    }
}
