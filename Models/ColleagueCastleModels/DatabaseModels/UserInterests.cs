using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;

namespace NewHorizon.Models.ColleagueCastleModels.DatabaseModels
{
    public class UserInterests
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "uid")]
        public string Uid { get; set; }

        public string PostId { get; set; }

        public List<string> InterestedUserIds { get; set; }

        public void AddUserInterest(string userId)
        {
            InterestedUserIds.Add(userId);
        }
    }
}
