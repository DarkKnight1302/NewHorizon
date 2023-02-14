namespace NewHorizon.Models.ColleagueCastleModels
{
    public class CreatePropertyPostResponse
    {
        public CreatePropertyPostResponse(string propertyPostId)
        {
            PropertyPostId = propertyPostId;
        }

        public string PropertyPostId { get; set; }
    }
}
