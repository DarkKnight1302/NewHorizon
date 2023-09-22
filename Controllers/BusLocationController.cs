using Microsoft.AspNetCore.Mvc;
using NewHorizon.Models;
using NewHorizon.Repositories.Interfaces;

namespace NewHorizon.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BusLocationController : ControllerBase
    {
        private readonly IBusLocationRepository _busLocationRepository;

        public BusLocationController(IBusLocationRepository busLocationRepository)
        {
            this._busLocationRepository = busLocationRepository;
        }

        [ApiExplorerSettings(GroupName = "v2")]
        [HttpGet("GetBusLocation")]
        public async Task<IActionResult> GetBusLocation(string busId)
        {
            if (string.IsNullOrWhiteSpace(busId)) 
            {
                return BadRequest("Invalid Bus ID");
            }

            var busLocation = await this._busLocationRepository.GetBusCoordinates(busId).ConfigureAwait(false);
            if (busLocation == null)
            {
                return NotFound("Location Not Found");
            }
            return Ok(busLocation);
        }

        [ApiExplorerSettings(GroupName = "v2")]
        [HttpPost("SaveBusLocation")]
        public async Task<IActionResult> SaveBusLocation(BusLocationRequest busLocationRequest)
        {
            if (busLocationRequest == null || string.IsNullOrEmpty(busLocationRequest.BusId))
            {
                return BadRequest("Invalid Bus ID");
            }
            if (busLocationRequest.Longitude == default(double) || busLocationRequest.Latitude == default(double)) 
            {
                return BadRequest("Invalid Lat / lng");
            }

            await this._busLocationRepository.SaveBusCoordindates(busLocationRequest.Latitude, busLocationRequest.Longitude, busLocationRequest.BusId);
            return Ok("Coordinates saved successfully");
        }
    }
}
