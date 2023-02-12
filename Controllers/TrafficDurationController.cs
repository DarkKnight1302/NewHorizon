using Microsoft.AspNetCore.Mvc;
using NewHorizon.Services.Interfaces;

namespace NewHorizon.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TrafficDurationController : ControllerBase
{
    private readonly ILogger<TrafficDurationController> _logger;
    private readonly ITrafficDataStorageService trafficDataStorageService;

    public TrafficDurationController(ILogger<TrafficDurationController> logger, ITrafficDataStorageService trafficDataStorageService)
    {
        _logger = logger;
        this.trafficDataStorageService = trafficDataStorageService;
    }

    [HttpGet(Name = "GetTrafficData")]
    public async Task<Dictionary<string, int>> Get(string fromPlaceId, string toPlaceId, int dayOfWeek) 
    {
        var response = await this.trafficDataStorageService.GetMedianRouteTimeAsync(fromPlaceId, toPlaceId, dayOfWeek).ConfigureAwait(false);
        return response;
    }
}
