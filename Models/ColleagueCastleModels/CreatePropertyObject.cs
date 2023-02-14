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
    }
}
