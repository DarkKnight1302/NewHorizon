namespace NewHorizon.Models.ColleagueCastleModels
{
    public class SignInResponse
    {
        public SignInResponse(string _sessionToken)
        {
            SessionToken = _sessionToken;
        }

        public string SessionToken { get; }
    }
}
