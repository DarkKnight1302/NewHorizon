using GoogleApi.Entities.Places.Common;
using Microsoft.AspNetCore.Mvc;
using NewHorizon.Models.ColleagueCastleModels;
using NewHorizon.Repositories.Interfaces;

namespace NewHorizon.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsernameController : ControllerBase
{
    private readonly ILogger<TrafficDurationController> _logger;
    private readonly IUserRepository userRepository;

    public UsernameController(ILogger<TrafficDurationController> logger, IUserRepository userRepository)
    {
        _logger = logger;
        this.userRepository = userRepository;
    }

    [ApiExplorerSettings(GroupName = "v1")]
    [HttpGet("username-availability")]
    public async Task<IActionResult> Get(string username) 
    {
        if (string.IsNullOrEmpty(username))
        {
            return BadRequest("Invalid Username");
        }
        User user = await this.userRepository.GetUserByUserNameAsync(username).ConfigureAwait(false);
        if (user == null)
        {
            return Ok(new UsernameAvailabilityResponse(username, true));
        }
        return Ok(new UsernameAvailabilityResponse(username, false));
    }
}
