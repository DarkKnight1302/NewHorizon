using NewHorizon.Models.ColleagueCastleModels.DatabaseModels;

namespace NewHorizon.Models.ColleagueCastleModels
{
    public class SearchPropertyResponse
    {
        public SearchPropertyResponse(IEnumerable<PropertyPostResponse> propertyPostDetails) 
        {
            this.propertyPostDetails = propertyPostDetails;
        }

        public IEnumerable<PropertyPostResponse> propertyPostDetails { get; set; }
    }
}
