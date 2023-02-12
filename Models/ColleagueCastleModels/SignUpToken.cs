using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;

namespace NewHorizon.Models.ColleagueCastleModels
{
    public class SignUpToken
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "uid")]
        public string Uid { get; set; }

        public string EmailAddress { get; set; }

        public string Token { get; set; }

        public DateTime Expiry { get; set; }
    }
}
