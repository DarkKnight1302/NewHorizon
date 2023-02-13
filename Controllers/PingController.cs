using GoogleApi.Entities.Places.Common;
using Microsoft.AspNetCore.Mvc;
using NewHorizon.Models;
using NewHorizon.Services.Interfaces;
using SkipTrafficLib.Services.Interfaces;

namespace NewHorizon.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PingController : ControllerBase
{
    private readonly ILogger<TrafficDurationController> _logger;

    public PingController(ILogger<TrafficDurationController> logger)
    {
        _logger = logger;
    }

    [ApiExplorerSettings(GroupName = "v1")]
    [HttpGet(Name = "ping")]
    public bool Get() 
    {
        return true;
    }
}
