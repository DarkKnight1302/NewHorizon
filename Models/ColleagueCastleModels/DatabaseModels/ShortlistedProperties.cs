using Newtonsoft.Json;

namespace NewHorizon.Models.ColleagueCastleModels.DatabaseModels
{
    public class ShortlistedProperties
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "uid")]
        public string Uid { get; set; }

        public string UserId { get; set; }

        public HashSet<string> ShortlistedPropertiesIds { get; set; }

        public void AddProperty(string propertyId)
        {
            if (ShortlistedPropertiesIds == null)
            {
                ShortlistedPropertiesIds = new HashSet<string>();
            }
            ShortlistedPropertiesIds.Add(propertyId);
        }
    }
}
