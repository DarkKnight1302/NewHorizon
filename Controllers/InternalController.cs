using Microsoft.AspNetCore.Mvc;
using NewHorizon.Models.ColleagueCastleModels;
using NewHorizon.Repositories.Interfaces;
using NewHorizon.Services.Interfaces;
using SkipTrafficLib.Services.Interfaces;
using System.Runtime.CompilerServices;

namespace NewHorizon.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InternalController : ControllerBase
    {
        private readonly IUserRepository userRepository;
        private readonly ISecretService secretService;
        private readonly IGooglePlaceService googlePlaceService;

        public InternalController(IUserRepository userRepository, ISecretService secretService, IGooglePlaceService googlePlaceService)
        {
            this.userRepository = userRepository;
            this.secretService = secretService;
            this.googlePlaceService = googlePlaceService;
        }

        [ApiExplorerSettings(GroupName = "v1")]
        [HttpPost("delete-user")]
        public async Task<IActionResult> DeleteUser([FromBody] DeleteUserRequest request)
        {
            if (request.SecretToken == null)
            {
                return BadRequest("Invalid secret token");
            }

            if (request.SecretToken == this.secretService.GetSecretValue("INTERNAL_SECRET_TOKEN"))
            {
                bool userDeleted = await this.userRepository.DeleteUser(request.UserId).ConfigureAwait(false);
                if (userDeleted)
                {
                    return Ok("User Deleted Successfully");
                }
            }

            return BadRequest("Unable to delete User");
        }

        [ApiExplorerSettings(GroupName = "v1")]
        [HttpPost("get-place-details")]
        public async Task<IActionResult> GetPlaceDetails([FromBody] GetPlaceDetailsRequest request)
        {
            if (request.SecretToken == null)
            {
                return BadRequest("Invalid secret token");
            }

            if (request.SecretToken == this.secretService.GetSecretValue("INTERNAL_SECRET_TOKEN"))
            {
                var details = await this.googlePlaceService.GetPlaceDetailsAsync(request.PlaceId).ConfigureAwait(false);
                return Ok(details);
            }

            return BadRequest("Not able to fetch details.");
        }
    }
}
