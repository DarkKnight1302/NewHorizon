using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;

namespace NewHorizon.Models.ColleagueCastleModels
{
    public class PropertyPostDetails
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "uid")]
        public string Uid { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string MapUrl { get; set; }

        public string FormattedAddress { get; set; }

        public List<string> Images { get; set; }

        public string CreatorUserName { get; set; }

        public long Views { get; set; }

        public List<string> InterestIds { get; set; }
    }
}
