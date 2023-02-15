namespace NewHorizon.Models.ColleagueCastleModels
{
    public class SearchPropertyResponse
    {
        public SearchPropertyResponse(IReadOnlyList<PropertyPostDetails> propertyPostDetails) 
        {
            this.propertyPostDetails = propertyPostDetails;
        }

        public IReadOnlyList<PropertyPostDetails> propertyPostDetails { get; set; }
    }
}
