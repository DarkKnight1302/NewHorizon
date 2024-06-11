using Newtonsoft.Json;

namespace NewHorizon.Models.ColleagueCastleModels.DatabaseModels
{
    public class RefreshToken
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "uid")]
        public string UserId { get; set; }

        public string Token { get; set; }

        public DateTime Expiry { get; set; }

        public DateTime CreatedAt { get; set; }

    }
}
