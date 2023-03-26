using Microsoft.AspNetCore.Mvc;
using NewHorizon.Models.ColleagueCastleModels;
using NewHorizon.Repositories.Interfaces;
using NewHorizon.Services.ColleagueCastleServices.Interfaces;
using NewHorizon.Utils;

namespace NewHorizon.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResetPasswordController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ISessionTokenManager sessionTokenManager;

        public ResetPasswordController(IUserRepository userRepository, ISessionTokenManager sessionTokenManager)
        {
            this._userRepository = userRepository;
            this.sessionTokenManager = sessionTokenManager;
        }

        [ApiKeyRequired]
        [ApiExplorerSettings(GroupName = "v1")]
        [HttpPost]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest resetPasswordRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var sessionToken = HttpContext.Request.Headers["X-Api-Key"];
            if (string.IsNullOrEmpty(sessionToken))
            {
                return BadRequest("Invalid Session token.");
            }

            bool validSession = await this.sessionTokenManager.ValidateSessionToken(resetPasswordRequest.Username, sessionToken);

            if (!validSession)
            {
                return Unauthorized("Invalid Session");
            }

            var user = await _userRepository.GetUserByUserNameAsync(resetPasswordRequest.Username).ConfigureAwait(false);

            if (user == null)
            {
                return Unauthorized("Incorrect Username.");
            }

            if (resetPasswordRequest.NewPassword == resetPasswordRequest.OldPassword)
            {
                return BadRequest("New password cannot be same as old password");
            }

            if (!HashingUtil.VerifyPassword(resetPasswordRequest.OldPassword, user.HashedPassword, user.Salt))
            {
                return Unauthorized("Incorrect old password");
            }

            bool success = await _userRepository.UpdateUserPassword(user, resetPasswordRequest.NewPassword).ConfigureAwait(false);
            if (!success)
            {
                return BadRequest("Something Went Wrong.");
            }

            return Ok("Password updated successfully.");
        }
    }
}
