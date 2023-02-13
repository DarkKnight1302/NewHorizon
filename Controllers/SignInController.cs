using Microsoft.AspNetCore.Mvc;
using NewHorizon.Models.ColleagueCastleModels;
using NewHorizon.Repositories.Interfaces;
using NewHorizon.Services.ColleagueCastleServices.Interfaces;
using NewHorizon.Utils;
using Newtonsoft.Json.Linq;

namespace NewHorizon.Controllers
{
    [Route("api/[controller]")]
    public class SignInController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly ISessionTokenManager sessionTokenManager;

        public SignInController(IUserRepository userRepository, ISessionTokenManager sessionTokenManager)
        {
            this._userRepository = userRepository;
            this.sessionTokenManager = sessionTokenManager;
        }

        [ApiExplorerSettings(GroupName = "v1")]
        [HttpPost]
        public async Task<IActionResult> SignIn([FromBody] LoginData loginData)
        {
            var user = await _userRepository.GetUserByUserNameAsync(loginData.Username).ConfigureAwait(false);
            if (user == null)
            {
                return Unauthorized("Incorrect Username or password");
            }

            if (!PasswordHashingUtil.VerifyPassword(loginData.Password, user.HashedPassword, user.Salt))
            {
                return Unauthorized("Incorrect Username or password");
            }
            string sessionToken = await this.sessionTokenManager.GenerateSessionToken(user.UserName).ConfigureAwait(false);
            return Ok(new SignInResponse(sessionToken));
        }
    }
}
