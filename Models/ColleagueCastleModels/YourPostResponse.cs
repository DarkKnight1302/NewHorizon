using NewHorizon.Models.ColleagueCastleModels.DatabaseModels;

namespace NewHorizon.Models.ColleagueCastleModels
{
    public class YourPostResponse
    {
        public YourPostResponse(IEnumerable<PropertyPostDetails> propertyPostDetails) 
        {
            this.PropertyPostDetails = propertyPostDetails;
        }

        public IEnumerable<PropertyPostDetails> PropertyPostDetails { get; set; }
    }
}
