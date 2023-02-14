﻿using GoogleApi.Entities.Places.Common;
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

    public BlobStorageController(ILogger<TrafficDurationController> logger, IBlobStorageService blobStorageService)
    {
        _logger = logger;
        this.blobStorageService = blobStorageService;
    }

    [ApiExplorerSettings(GroupName = "v1")]
    [HttpPost("fetch-blob-sas-token")]
    public async Task<IActionResult> Get(string username) 
    {
        return Ok();
        /*if (string.IsNullOrEmpty(username))
        {
            return BadRequest("Invalid Username");
        }
        User user = await this.userRepository.GetUserByUserNameAsync(username).ConfigureAwait(false);
        if (user == null)
        {
            return Ok(new UsernameAvailabilityResponse(username, true));
        }
        return Ok(new UsernameAvailabilityResponse(username, false));*/
    }
}
