using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NewHorizon.Helpers;
using Newtonsoft.Json;

namespace NewHorizon.Models
{
    public class RouteDbData
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "uid")]
        public string Uid { get; set; }

        public Dictionary<string, List<int>> routeTimeData { get; set; } = new Dictionary<string, List<int>>();
    }
}
