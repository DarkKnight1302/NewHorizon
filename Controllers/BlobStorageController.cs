using GoogleApi.Entities.Places.Common;
using Microsoft.AspNetCore.Mvc;
using NewHorizon.Models.ColleagueCastleModels;
using NewHorizon.Repositories.Interfaces;
using NewHorizon.Services.ColleagueCastleServices.Interfaces;

namespace NewHorizon.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BlobStorageController : ControllerBase
{
    private readonly ILogger<TrafficDurationController> _logger;
    private readonly IBlobStorageService blobStorageService;
    private readonly ISessionTokenManager sessionTokenManager;

    public BlobStorageController(ILogger<TrafficDurationController> logger, IBlobStorageService blobStorageService, ISessionTokenManager sessionTokenManager)
    {
        _logger = logger;
        this.blobStorageService = blobStorageService;
        this.sessionTokenManager = sessionTokenManager;
    }

    [ApiExplorerSettings(GroupName = "v1")]
    [HttpPost("fetch-blob-sas-token")]
    public async Task<IActionResult> Get([FromBody] FetchBlobSasTokenRequest fetchBlobSasTokenRequest)
    {
        bool sessionValid = await this.sessionTokenManager.ValidateSessionToken(fetchBlobSasTokenRequest.UserId, fetchBlobSasTokenRequest.SessionId).ConfigureAwait(false);
        if (!sessionValid)
        {
            return BadRequest("Invalid Session Token");
        }
        string sasToken = this.blobStorageService.GenerateBlobStorageAccessToken();
        return Ok(sasToken);
    }
}
