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

        public List<string> Images { get; set; }

        public int RentAmount { get; set; }

        public PropertyType PropertyType { get; set; }

        public FlatType FlatType { get; set; }

        public FurnishingType FurnishingType { get; set; }

        public TenantPreference TenantPreference { get; set; }

        public FoodPreference FoodPreference { get; set; }

        public Smoking Smoking { get; set; }

        public Drinking Drinking { get; set; }

        public ExperienceRange ExperienceRange { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum PropertyType
    {
        Room,
        Flat,
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum FlatType
    {
        BHK1 = 0,
        BHK2 = 1,
        BHK3 = 2,
        OTHER = 3,
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum FurnishingType
    {
        UnFurnished = 0,
        SemiFurnished = 1,
        FullyFurnished = 2,
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum TenantPreference
    {
        Family = 0,
        Men = 1,
        Women = 2,
        NoPreference = 3,
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum FoodPreference
    {
        None = 0,
        Veg = 1,
        NonVeg = 2,
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum Smoking
    {
        Allowed = 0,
        NotAllowed = 1,
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum Drinking
    {
        Allowed = 0,
        NotAllowed = 1,
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public class ExperienceRange
    {
        public int MinExpYears { get; set; }
        public int MaxExpYears { get; set; }
    }

}
