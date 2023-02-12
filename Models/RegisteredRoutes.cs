using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;

namespace NewHorizon.Models
{
    public class RegisteredRoutes
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "uid")]
        public string Uid { get; set; }

        public string fromPlaceId { get; set; }

        public string toPlaceId { get; set; }

        public List<Tuple<DateTimeOffset, DateTimeOffset>> registeredTimeSlots { get; set;} = new List<Tuple<DateTimeOffset, DateTimeOffset>>();
    }
}
