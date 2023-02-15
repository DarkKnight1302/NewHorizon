using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;

namespace NewHorizon.Models.ColleagueCastleModels
{
    public class PropertyPost
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "uid")]
        public string Uid { get; set; }

        public bool Available { get; set; }

        public string PlaceId { get; set; }

        public string City { get; set; }

        public string Company { get; set; }
    }

    public class Location
    {
        public Location(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
