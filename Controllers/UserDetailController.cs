using Microsoft.AspNetCore.Mvc;
using NewHorizon.Models.ColleagueCastleModels;
using NewHorizon.Models.ColleagueCastleModels.DatabaseModels;
using NewHorizon.Repositories.Interfaces;
using NewHorizon.Services.ColleagueCastleServices.Interfaces;

namespace NewHorizon.Controllers
{
    [Route("api/[controller]")]
    public class UserDetailController : ControllerBase
    {
        private readonly IPropertyPostService _propertyPostService;
        private readonly ISessionTokenManager _sessionTokenManager;
        private readonly IUserRepository _userRepository;

        public UserDetailController(
            IPropertyPostService propertyPostService,
            ISessionTokenManager sessionTokenManager,
            IUserRepository userRepository)
        {
            this._propertyPostService = propertyPostService;
            this._sessionTokenManager = sessionTokenManager;
            this._userRepository = userRepository;
        }

        [ApiKeyRequired]
        [ApiExplorerSettings(GroupName = "v1")]
        [HttpGet("get-userdetails")]
        public async Task<IActionResult> GetUserDetails(string propertyPostId)
        {
            var sessionId = HttpContext.Request.Headers["X-Api-Key"];
            if (string.IsNullOrEmpty(sessionId))
            {
                return BadRequest("Invalid sessionId");
            }
            bool isValidToken = await this._sessionTokenManager.ValidateSessionToken(sessionId).ConfigureAwait(false);
            if (!isValidToken)
            {
                return BadRequest("Invalid session Id");
            }
            if (string.IsNullOrEmpty(propertyPostId))
            {
                return BadRequest("Invalid property Post Id");
            }
            PropertyPostDetails propertyPostDetails = await this._propertyPostService.GetPropertyPostAsync(propertyPostId).ConfigureAwait(false);
            if (propertyPostDetails == null)
            {
                return NotFound("No property found");
            }
            string creatorUserId = propertyPostDetails.CreatorUserName;
            var user = await this._userRepository.GetUserByUserNameAsync(creatorUserId).ConfigureAwait(false);
            if (user == null)
            {
                return NotFound("No User found");
            }
            UserDetails userDetails = new UserDetails()
            {
                Name = user.Name,
                Email = user.Email,
                Company = user.Company,
                Phone = user.PhoneNumber,
                ExperienceInYears = user.ExperienceInYears
            };
            return Ok(userDetails);
        }
    }
}
