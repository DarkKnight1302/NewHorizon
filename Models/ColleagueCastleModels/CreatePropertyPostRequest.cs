using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace NewHorizon.Models.ColleagueCastleModels
{
    public class CreatePropertyPostRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string SessionId { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string PlaceId { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 1)]
        public string Title { get; set; }

        [Required]
        [StringLength(5000, MinimumLength = 1)]
        public string Description { get; set; }

        public List<string> Images { get; set; } = new List<string>();

        [Required]
        public int RentAmount { get; set; }

        [Required]
        [JsonProperty("propertyType")]
        public PropertyType PropertyType { get; set; }

        [Required]
        [JsonProperty("FlatType")]
        public FlatType FlatType { get; set; }

        [Required]
        public FurnishingType FurnishingType { get; set; }

        [Required]
        public TenantPreference TenantPreference { get; set; }

        [Required]
        public FoodPreference FoodPreference { get; set; }

        [Required]
        public Smoking Smoking { get; set; }

        [Required]
        public Drinking Drinking { get; set; }

        public ExperienceRange ExperienceRange { get; set; } = new ExperienceRange();
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum PropertyType
    {
        Room,
        Flat,
        Ignore,
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum FlatType
    {
        BHK1,
        BHK2,
        BHK3,
        OTHER,
        Ignore
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum FurnishingType
    {
        UnFurnished = 0,
        SemiFurnished = 1,
        FullyFurnished = 2,
        Ignore
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum TenantPreference
    {
        Family = 0,
        Men = 1,
        Women = 2,
        NoPreference = 3,
        Ignore,
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum FoodPreference
    {
        None = 0,
        Veg = 1,
        Ignore = 2,
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum Smoking
    {
        Allowed = 0,
        NotAllowed = 1,
        Ignore,
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum Drinking
    {
        Allowed = 0,
        NotAllowed = 1,
        Ignore,
    }

    public class ExperienceRange
    {
        public int MinExpYears { get; set; } = 0;
        public int MaxExpYears { get; set; } = 50;
    }

}
