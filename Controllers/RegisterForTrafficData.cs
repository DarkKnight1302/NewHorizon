using Microsoft.AspNetCore.Mvc;
using NewHorizon.Handlers;

namespace NewHorizon.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterForTrafficDataController : ControllerBase
    {
        private readonly IRouteRegistrationHandler routeRegistrationHandler;

        public RegisterForTrafficDataController(IRouteRegistrationHandler routeRegistrationHandler)
        {
            this.routeRegistrationHandler = routeRegistrationHandler;
        }

        [ApiExplorerSettings(GroupName = "v2")]
        [HttpGet(Name = "RegisterForTrafficData")]
        public async Task<IActionResult> Get(string fromPlaceId, string toPlaceId, DateTimeOffset startTimeUtc, DateTimeOffset endTimeUtc, string region)
        {
            if (region != "IN")
            {
                return BadRequest("Region not supported");
            }
            if (endTimeUtc - startTimeUtc > TimeSpan.FromHours(3))
            {
                return BadRequest("Time range should be less than 3 hours");
            }
            await this.routeRegistrationHandler.RegisterRoute(fromPlaceId, toPlaceId, startTimeUtc, endTimeUtc).ConfigureAwait(false);
            return Ok("Successfully registered");
        }
    }
}
