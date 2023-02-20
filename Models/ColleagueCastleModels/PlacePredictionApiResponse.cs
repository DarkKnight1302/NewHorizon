namespace NewHorizon.Models.ColleagueCastleModels
{
    public class PlacePredictionApiResponse
    {
        public PlacePredictionApiResponse(string placeId, string desc)
        {
            this.PlaceId = placeId;
            this.Description = desc;
        }

        public string PlaceId { get; set; }

        public string Description { get; set; }
    }
}
