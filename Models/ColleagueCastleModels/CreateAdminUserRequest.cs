namespace NewHorizon.Models.ColleagueCastleModels
{
    public class CreateAdminUserRequest
    {
        public string SecretToken { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }
    }
}
