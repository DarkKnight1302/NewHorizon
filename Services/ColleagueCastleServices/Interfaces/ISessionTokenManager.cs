namespace NewHorizon.Services.ColleagueCastleServices.Interfaces
{
    public interface ISessionTokenManager
    {
        public Task<string> GenerateSessionToken(string userId);

        public Task<bool> ValidateSessionToken(string userId, string sessionToken);
    }
}
