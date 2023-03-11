using NewHorizon.Models.ColleagueCastleModels;

namespace NewHorizon.Services.ColleagueCastleServices.Interfaces
{
    public interface IGoogleSignInService
    {
        public Task<ValidateGoogleTokenResponse> ValidateToken(string token);
    }
}
