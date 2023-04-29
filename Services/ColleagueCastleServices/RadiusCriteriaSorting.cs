using GeoCoordinatePortable;
using GoogleApi.Entities.Places.Details.Response;
using NewHorizon.Models.ColleagueCastleModels;
using NewHorizon.Models.ColleagueCastleModels.DatabaseModels;
using NewHorizon.Services.ColleagueCastleServices.Interfaces;
using SkipTrafficLib.Services.Interfaces;

namespace NewHorizon.Services.ColleagueCastleServices
{
    public class RadiusCriteriaSorting : ICriteriaSorting
    {
        private IGooglePlaceService _googlePlaceService;

        public RadiusCriteriaSorting(IGooglePlaceService googlePlaceService) 
        {
            this._googlePlaceService = googlePlaceService;
        }

        public async Task Sort(List<PropertyPostResponse> propertyPostResponses, SearchPropertyRequest searchPropertyRequest)
        {
            DetailsResult placeDetails = await this._googlePlaceService.GetPlaceDetailsAsync(searchPropertyRequest.OfficePlaceId).ConfigureAwait(false);
            double originLat = placeDetails.Geometry.Location.Latitude;
            double originLng = placeDetails.Geometry.Location.Longitude;
            var comparer = new RadialComparer(originLat, originLng);
            if (propertyPostResponses.Count == 1)
            {
                var originCoordinate = new GeoCoordinate(originLat, originLng);
                var XCoordindate = new GeoCoordinate(propertyPostResponses[0].Location.coordinates[1], propertyPostResponses[0].Location.coordinates[0]);
                propertyPostResponses[0].RadialDistance = (int)originCoordinate.GetDistanceTo(XCoordindate);
            }
            else
            {
                propertyPostResponses.Sort(comparer);
            }
        }
    }

    public class RadialComparer : IComparer<PropertyPostResponse>
    {
        private GeoCoordinate originCoordinate;

        public RadialComparer(double originLat, double originLng)
        {
            this.originCoordinate = new GeoCoordinate(originLat, originLng);
        }
        public int Compare(PropertyPostResponse? x, PropertyPostResponse? y)
        {
            var XCoordindate = new GeoCoordinate(x.Location.coordinates[1], x.Location.coordinates[0]);
            var YCoordindate = new GeoCoordinate(y.Location.coordinates[1], y.Location.coordinates[0]);
            var distanceX = originCoordinate.GetDistanceTo(XCoordindate);
            var distanceY = originCoordinate.GetDistanceTo(YCoordindate);
            x.RadialDistance = (int)distanceX;
            y.RadialDistance = (int)distanceY;
            return distanceX.CompareTo(distanceY);
        }
    }
}
