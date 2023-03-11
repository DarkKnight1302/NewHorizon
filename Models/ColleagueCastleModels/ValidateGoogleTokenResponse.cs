namespace NewHorizon.Models.ColleagueCastleModels
{
    public class ValidateGoogleTokenResponse
    {
        public bool IsRegisteredUser { get; set; }

        public string UserId { get; set; }

        public string Name { get; set; }

        public string token { get; set; }
    }
}
