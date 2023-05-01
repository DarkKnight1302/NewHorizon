using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NewHorizon.Models.ColleagueCastleModels;
using NewHorizon.Repositories.Interfaces;
using NewHorizon.Services.ColleagueCastleServices.Interfaces;
using NewHorizon.Utils;

namespace NewHorizon.Controllers
{
    [Route("api/[controller]")]
    public class SignInController : Controller
    {
        private const int MaxFailedAttempts = 10;
        private readonly IUserRepository _userRepository;
        private readonly ISessionTokenManager sessionTokenManager;
        private readonly IMemoryCache _memoryCache;

        public SignInController(IUserRepository userRepository, ISessionTokenManager sessionTokenManager, IMemoryCache memoryCache)
        {
            this._userRepository = userRepository;
            this.sessionTokenManager = sessionTokenManager;
            this._memoryCache = memoryCache;
        }

        [ApiKeyRequired]
        [ApiExplorerSettings(GroupName = "v1")]
        [HttpPost]
        public async Task<IActionResult> SignIn([FromBody] LoginData loginData)
        {
            var userId = HttpContext.Request.Headers["X-Api-Key"];
            if (string.IsNullOrEmpty(userId) || loginData.Username != userId)
            {
                return BadRequest("Invalid User Id");
            }
            int failedLoginAttempts = this._memoryCache.GetOrCreate(userId, e =>
            {
                e.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                return 0;
            });
            if (failedLoginAttempts > MaxFailedAttempts)
            {
                return new ObjectResult("Too many attempts") { StatusCode = 429 };
            }
            var user = await _userRepository.GetUserByUserNameAsync(loginData.Username).ConfigureAwait(false);
            if (user == null)
            {
                return Unauthorized("Incorrect Username or password");
            }

            if (!HashingUtil.VerifyPassword(loginData.Password, user.HashedPassword, user.Salt))
            {
                this._memoryCache.Set(userId, failedLoginAttempts + 1, TimeSpan.FromHours(1));
                return Unauthorized("Incorrect Username or password");
            }
            string sessionToken = await this.sessionTokenManager.GenerateSessionToken(user.UserName).ConfigureAwait(false);
            return Ok(new SignInResponse(sessionToken));
        }
    }
}
