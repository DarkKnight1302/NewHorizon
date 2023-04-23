using GeoCoordinatePortable;
using GoogleApi.Entities.Places.Details.Response;
using NewHorizon.Models.ColleagueCastleModels;
using NewHorizon.Models.ColleagueCastleModels.DatabaseModels;
using NewHorizon.Services.ColleagueCastleServices.Interfaces;
using SkipTrafficLib.Services.Interfaces;

namespace NewHorizon.Services.ColleagueCastleServices
{
    public class RadiusCriteriaMatching : ICriteriaMatching
    {
        private readonly IGooglePlaceService googlePlaceService;

        public RadiusCriteriaMatching(IGooglePlaceService googlePlaceService) 
        {
            this.googlePlaceService = googlePlaceService;
        }
        public async Task<bool> IsMatch(PropertyPostDetails propertyPostDetails, SearchPropertyRequest searchPropertyRequest)
        {
            /* if (searchPropertyRequest.SearchRadiusInKm == 0)
             {
                 return true;
             }

             DetailsResult placeDetails = await this.googlePlaceService.GetPlaceDetailsAsync(searchPropertyRequest.OfficePlaceId).ConfigureAwait(false);
             double originLat = placeDetails.Geometry.Location.Latitude;
             double originLng = placeDetails.Geometry.Location.Longitude;

             var originCoordinate = new GeoCoordinate(originLat, originLng);
             var destCooridinate = new GeoCoordinate(destinationLat, destinationLng);

             double distanceInMeters = originCoordinate.GetDistanceTo(destCooridinate);

             double limitDistanceInMeters = (double)searchPropertyRequest.SearchRadiusInKm * 1000;

             return distanceInMeters <= limitDistanceInMeters;*/
            await Task.CompletedTask;
            return true;
        }
    }
}
