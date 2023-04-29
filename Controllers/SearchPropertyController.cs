using Microsoft.AspNetCore.Mvc;
using NewHorizon.Models.ColleagueCastleModels;
using NewHorizon.Services.ColleagueCastleServices.Interfaces;

namespace NewHorizon.Controllers
{
    [Route("api/[controller]")]
    public class SearchPropertyController : Controller
    {
        private readonly ISessionTokenManager sessionTokenManager;
        private readonly ISearchPropertyService searchPropertyService;

        public SearchPropertyController(
            ISessionTokenManager sessionTokenManager,
            ISearchPropertyService searchPropertyService)
        {
            this.sessionTokenManager = sessionTokenManager;
            this.searchPropertyService = searchPropertyService;
        }

        [ApiExplorerSettings(GroupName = "v1")]
        [HttpPost]
        public async Task<IActionResult> SearchPropertyPost([FromBody] SearchPropertyRequest searchPropertyRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            bool valid = await this.sessionTokenManager.ValidateSessionToken(searchPropertyRequest.UserId, searchPropertyRequest.SessionId);
            if (!valid)
            {
                return BadRequest("Invalid Session token");
            }

            (IEnumerable<PropertyPostResponse>? propertyDetails, string error) propertyPostDetails = await this.searchPropertyService.GetMatchingPropertyListAsync(searchPropertyRequest).ConfigureAwait(false);

            if (propertyPostDetails.propertyDetails == null)
            {
                return BadRequest(propertyPostDetails.error);
            }

            return Ok(new SearchPropertyResponse(propertyPostDetails.propertyDetails));
        }
    }
}
