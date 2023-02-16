using Microsoft.AspNetCore.Mvc;
using NewHorizon.Services.ColleagueCastleServices.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class BlobController : ControllerBase
{
    private readonly IBlobStorageService blobStorageService;
    private readonly ISessionTokenManager sessionTokenManager;

    public BlobController(IBlobStorageService blobStorageService, ISessionTokenManager sessionTokenManager)
    {
        this.blobStorageService = blobStorageService;
        this.sessionTokenManager = sessionTokenManager;
    }

    [ApiKeyRequired]
    [HttpPost("Upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        try
        {
            var sessionId = HttpContext.Request.Headers["X-Api-Key"];
            if (string.IsNullOrEmpty(sessionId))
            {
                return BadRequest("Invalid Session Token");
            }
            bool valid = await this.sessionTokenManager.ValidateSessionToken(sessionId);

            if (!valid)
            {
                return BadRequest("Invalid Session token");
            }

            if (file == null || file.Length == 0)
            {
                return BadRequest("No file selected.");
            }
            
            string blobUrl = await this.blobStorageService.UploadFileToBlobAsync(file, file.FileName);
            return Ok(blobUrl);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}
