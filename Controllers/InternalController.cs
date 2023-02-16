using Microsoft.AspNetCore.Mvc;
using NewHorizon.Models.ColleagueCastleModels;
using NewHorizon.Repositories.Interfaces;
using NewHorizon.Services.Interfaces;

namespace NewHorizon.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InternalController : ControllerBase
    {
        private readonly IUserRepository userRepository;
        private readonly ISecretService secretService;

        public InternalController(IUserRepository userRepository, ISecretService secretService)
        {
            this.userRepository = userRepository;
            this.secretService = secretService;
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
    }
}
