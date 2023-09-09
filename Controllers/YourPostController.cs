using Microsoft.AspNetCore.Mvc;
using NewHorizon.Models.ColleagueCastleModels;
using NewHorizon.Models.ColleagueCastleModels.DatabaseModels;
using NewHorizon.Services.ColleagueCastleServices.Interfaces;

namespace NewHorizon.Controllers
{
    [Route("api/[controller]")]
    public class YourPostController : Controller
    {
        private readonly IPropertyPostService propertyPostService;
        private readonly ISessionTokenManager sessionTokenManager;

        public YourPostController(IPropertyPostService propertyPostService, ISessionTokenManager sessionTokenManager)
        {
            this.propertyPostService = propertyPostService;
            this.sessionTokenManager = sessionTokenManager;
        }

        [RequireHttps]
        [ApiExplorerSettings(GroupName = "v1")]
        [HttpPost("get-post")]
        public async Task<IActionResult> GetYourPost([FromBody] GetYourPostRequest GetYourPostRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            bool isValid = await this.sessionTokenManager.ValidateSessionToken(GetYourPostRequest.UserId, GetYourPostRequest.SessionId);
            if (!isValid)
            {
                return BadRequest("Invalid Session token");
            }

            IEnumerable<PropertyPostDetails> propertyPostDetails = await this.propertyPostService.GetUserPropertyPostsAsync(GetYourPostRequest.UserId).ConfigureAwait(false);

            return Ok(new YourPostResponse(propertyPostDetails));
        }

        [RequireHttps]
        [ApiExplorerSettings(GroupName = "v1")]
        [HttpPost("delete-post")]
        public async Task<IActionResult> DeleteYourPost([FromBody] DeleteYourPostRequest deleteYourPostRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            bool isValid = await this.sessionTokenManager.ValidateSessionToken(deleteYourPostRequest.UserId, deleteYourPostRequest.SessionToken);
            if (!isValid)
            {
                return BadRequest("Invalid Session token");
            }

            bool success = await this.propertyPostService.DeletePropertyPostAsync(deleteYourPostRequest.PostId, deleteYourPostRequest.UserId).ConfigureAwait(false);
            if (success)
            {
                return Ok("Successfully Deleted");
            }
            return BadRequest("Unable to delete Property");
        }
    }
}
