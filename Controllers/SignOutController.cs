using Microsoft.AspNetCore.Mvc;
using NewHorizon.Models.ColleagueCastleModels;
using NewHorizon.Repositories.Interfaces;
using NewHorizon.Services.ColleagueCastleServices.Interfaces;
using NewHorizon.Utils;

namespace NewHorizon.Controllers
{
    [Route("api/[controller]")]
    public class SignOutController : Controller
    {
        private readonly ISessionTokenManager sessionTokenManager;

        public SignOutController(ISessionTokenManager sessionTokenManager)
        {
            this.sessionTokenManager = sessionTokenManager;
        }

        [ApiKeyRequired]
        [ApiExplorerSettings(GroupName = "v1")]
        [HttpPost]
        public async Task<IActionResult> SignOut([FromBody] SignOutRequest signOutRequest)
        {
            var sessionToken = HttpContext.Request.Headers["X-Api-Key"];
            if (string.IsNullOrEmpty(signOutRequest.UserId))
            {
                return BadRequest("Invalid User Id");
            }
            if (sessionToken != signOutRequest.SessionToken)
            {
                return BadRequest("Invalid Session Token in Header");
            }
            bool isValidToken = await this.sessionTokenManager.ValidateSessionToken(signOutRequest.UserId, signOutRequest.SessionToken).ConfigureAwait(false);
            if (!isValidToken)
            {
                return BadRequest("Invalid Session Token");
            }
            this.sessionTokenManager.DeleteUserSession(signOutRequest.UserId, signOutRequest.SessionToken);
            return Ok("SignOut Successful");
        }
    }
}
