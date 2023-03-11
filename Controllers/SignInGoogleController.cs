using Microsoft.AspNetCore.Mvc;
using NewHorizon.Models.ColleagueCastleModels;
using NewHorizon.Services.ColleagueCastleServices.Interfaces;

namespace NewHorizon.Controllers
{
    [Route("api/[controller]")]
    public class SignInGoogleController : Controller
    {
        private readonly IGoogleSignInService googleSignInService;

        public SignInGoogleController(IGoogleSignInService googleSignInService)
        {
            this.googleSignInService = googleSignInService;
        }

        [ApiExplorerSettings(GroupName = "v1")]
        [HttpPost]
        public async Task<IActionResult> SignIn([FromBody] GoogleSignInRequest request)
        {
            var response = await this.googleSignInService.ValidateToken(request.Token);
            if (response == null)
            {
                return BadRequest("Something went wrong");
            }
            return Ok(response);
        }
    }
}
