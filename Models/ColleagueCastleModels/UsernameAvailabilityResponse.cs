namespace NewHorizon.Models.ColleagueCastleModels
{
    public class UsernameAvailabilityResponse
    {
        public UsernameAvailabilityResponse(string username, bool isUsernameAvailable)
        {
            Username = username;
            IsUsernameAvailable = isUsernameAvailable;
        }

        public string Username { get; set; }

        public bool IsUsernameAvailable { get; set; }
    }
}
