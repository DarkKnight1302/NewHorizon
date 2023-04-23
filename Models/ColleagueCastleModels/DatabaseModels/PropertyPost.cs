using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;

namespace NewHorizon.Models.ColleagueCastleModels.DatabaseModels
{
    public class PropertyPost
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "uid")]
        public string Uid { get; set; }

        public bool Available { get; set; }

        public string PlaceId { get; set; }

        public Location Location { get; set; }

        public string City { get; set; }

        public string Company { get; set; }

        public DateTime CreatedAt { get; set; }
    }

    public class Location
    {
        public Location(double latitude, double longitude)
        {
            this.coordinates[0] = longitude;
            this.coordinates[1] = latitude;
            this.type = "Point";
        }

        public string type { get; set; }
        
        public double[] coordinates { get; set; } = new double[2];
    }
}
