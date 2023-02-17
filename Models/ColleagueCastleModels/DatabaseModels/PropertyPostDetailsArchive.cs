using Newtonsoft.Json;

namespace NewHorizon.Models.ColleagueCastleModels.DatabaseModels
{
    public class PropertyPostDetailsArchive
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "uid")]
        public string Uid { get; set; }

        public string PostId { get; set; }

        public PropertyPostDetails PropertyPostDetails { get; set; }

        public PropertyPost PropertyPost { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
