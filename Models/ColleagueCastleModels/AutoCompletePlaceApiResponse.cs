using GoogleApi.Entities.Places.Common;

namespace NewHorizon.Models.ColleagueCastleModels
{
    public class AutoCompletePlaceApiResponse
    {
        public IEnumerable<PlacePrediction> predictions { get; set; }

        public string status { get; set; }
    }
}
