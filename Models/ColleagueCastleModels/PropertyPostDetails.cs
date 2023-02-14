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

        public int RentAmount { get; set; }

        public PropertyType PropertyType { get; set; }

        public FlatType FlatType { get; set; }

        public FurnishingType FurnishingType { get; set; }

        public TenantPreference TenantPreference { get; set; }

        public FoodPreference FoodPreference { get; set; }

        public Smoking Smoking { get; set; }

        public Drinking Drinking { get; set; }

        public ExperienceRange ExperienceRange { get; set; }

        public long Views { get; set; }

        public List<string> InterestIds { get; set; }
    }
}
