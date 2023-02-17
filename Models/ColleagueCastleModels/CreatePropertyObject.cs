using NewHorizon.Models.ColleagueCastleModels.DatabaseModels;

namespace NewHorizon.Models.ColleagueCastleModels
{
    public class CreatePropertyObject
    {
        public string username { get; set; }

        public string placeId { get; set; }

        public Location location { get; set; }
        public string city { get; set; }

        public string company { get; set; }

        public string title { get; set; }

        public string description { get; set; }

        public string MapUrl { get; set; }

        public string FormattedAddress { get; set; }

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
}
