using Microsoft.AspNetCore.Mvc;
using NewHorizon.Models.ColleagueCastleModels.DatabaseModels;
using NewHorizon.Services.ColleagueCastleServices.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class InterestController : ControllerBase
{
    private readonly ISessionTokenManager sessionTokenManager;
    private readonly IInterestService interestService;

    public InterestController(ISessionTokenManager sessionTokenManager, IInterestService interestService)
    {
        this.sessionTokenManager = sessionTokenManager;
        this.interestService = interestService;
    }

    [RequireHttps]
    [ApiKeyRequired]
    [HttpPost("show-interest")]
    public async Task<IActionResult> ShowInterest([FromBody] ShowInterestRequest showInterestRequest)
    {
        var sessionId = HttpContext.Request.Headers["X-Api-Key"];
        if (string.IsNullOrEmpty(sessionId))
        {
            return BadRequest("Invalid Session Token");
        }
        bool valid = await this.sessionTokenManager.ValidateSessionToken(showInterestRequest.UserId, sessionId).ConfigureAwait(false);
        if (!valid)
        {
            return BadRequest("Invalid token");
        }
        this.interestService.ShowInterestInPost(showInterestRequest.PostId, showInterestRequest.UserId);
        return Ok("Interest Registered in Property");
    }
}
