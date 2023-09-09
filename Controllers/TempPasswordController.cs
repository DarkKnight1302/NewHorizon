using Microsoft.AspNetCore.Mvc;
using NewHorizon.Services.Interfaces;

namespace NewHorizon.Controllers
{
    [Route("api/[controller]")]
    public class TempPasswordController : Controller
    {
        private readonly IGenerateAndSendPasswordService generateAndSendPasswordService;

        public TempPasswordController(IGenerateAndSendPasswordService generateAndSendPasswordService)
        {
            this.generateAndSendPasswordService = generateAndSendPasswordService;
        }

        [RequireHttps]
        [ApiKeyRequired]
        [ApiExplorerSettings(GroupName = "v1")]
        [HttpGet("generate-send-password")]
        public async Task<IActionResult> GenerateAndSendPassword()
        {
            string UserId = HttpContext.Request.Headers["X-Api-Key"];
            if (string.IsNullOrEmpty(UserId))
            {
                return BadRequest("Invalid Key");
            }
            bool success = await this.generateAndSendPasswordService.GenerateAndSendPassword(UserId).ConfigureAwait(false);
            if (!success)
            {
                return BadRequest("Something Went Wrong");
            }
            return Ok();
        }
    }
}
