using Google.Apis.Auth;
using NewHorizon.Models.ColleagueCastleModels;
using NewHorizon.Repositories.Interfaces;
using NewHorizon.Services.ColleagueCastleServices.Interfaces;

namespace NewHorizon.Services.ColleagueCastleServices
{
    public class GoogleSignInService : IGoogleSignInService
    {
        private readonly IUserRepository userRepository;
        private readonly ISessionTokenManager sessionTokenManager;
        private readonly ISignUpTokenService signUpTokenService;
        private readonly GoogleJsonWebSignature.ValidationSettings validationSettings;

        public GoogleSignInService(IUserRepository userRepository, ISessionTokenManager sessionTokenManager, ISignUpTokenService signUpTokenService)
        {
            this.userRepository = userRepository;
            this.sessionTokenManager = sessionTokenManager;
            this.signUpTokenService = signUpTokenService;
            this.validationSettings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[]
                    {
                        "991574871993-r66mcqkeejl05d21ioe0lbij0aif3af5.apps.googleusercontent.com"
                    }
            };
        }

        public async Task<ValidateGoogleTokenResponse> ValidateToken(string token)
        {
            try
            {
                GoogleJsonWebSignature.Payload payload = await GoogleJsonWebSignature.ValidateAsync(token, validationSettings);
                if (payload != null)
                {
                    string userId = payload.Email.ToLower();
                    var user = await this.userRepository.GetUserByUserNameAsync(userId);
                    if (user != null)
                    {
                        string sessionToken = await this.sessionTokenManager.GenerateSessionToken(user.UserName).ConfigureAwait(false);
                        return new ValidateGoogleTokenResponse
                        {
                            UserId = userId,
                            IsRegisteredUser = true,
                            token = sessionToken
                        };
                    }
                    string signUpToken = await this.signUpTokenService.GenerateSignUpTokenAsync(userId);
                    return new ValidateGoogleTokenResponse
                    {
                        IsRegisteredUser = false,
                        UserId = userId,
                        Name = payload.Name,
                        token = signUpToken
                    };
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
