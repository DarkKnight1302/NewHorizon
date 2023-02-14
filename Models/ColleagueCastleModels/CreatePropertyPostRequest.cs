namespace NewHorizon.Models.ColleagueCastleModels
{
    public class CreatePropertyPostRequest
    {
        public string SessionId { get; set; }

        public string PlaceId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public List<string> Images { get; set; }
    }
}
