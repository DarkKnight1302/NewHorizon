using System.ComponentModel.DataAnnotations;

namespace NewHorizon.Models.ColleagueCastleModels
{
    public class SearchPropertyRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string SessionId { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 1)]
        public string UserId { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 1)]
        public string OfficePlaceId { get; set; }

        public int SearchRadiusInKm { get; set; }

        public int MaximumRent { get; set; }

        public PropertyType PropertyType { get; set; }

        public FlatType FlatType { get; set; }

        public FurnishingType FurnishingType { get; set; }

        public TenantPreference TenantPreference { get; set; }

        public FoodPreference FoodPreference { get; set; }

        public Smoking Smoking { get; set; }

        public Drinking Drinking { get; set; }

        public int ExperienceInYears { get; set; }
    }
}
