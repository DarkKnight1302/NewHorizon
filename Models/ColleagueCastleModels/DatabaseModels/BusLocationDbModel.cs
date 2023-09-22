using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;

namespace NewHorizon.Models.ColleagueCastleModels.DatabaseModels
{
    public class BusLocationDbModel
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "uid")]
        public string BusId { get; set; }

        public BusLocation BusLocation { get; set; }
    }
}
