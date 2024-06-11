namespace NewHorizon.Services.ColleagueCastleServices.Interfaces
{
    public interface IRefreshTokenManager
    {
        public Task<string> GenerateRefreshTokenAsync(string userId);

        public Task<string> GenerateSessionTokenFromRefreshToken(string refreshToken);
    }
}
