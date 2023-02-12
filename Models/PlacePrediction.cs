namespace NewHorizon.Models
{
    public class PlacePrediction
    {
        public PlacePrediction(string placeId, string desc) 
        {
            this.PlaceId = placeId;
            this.Description = desc;
        }

        public string PlaceId { get; set; }

        public string Description { get; set; }
    }
}
