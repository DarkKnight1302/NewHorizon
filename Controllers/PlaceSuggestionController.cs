﻿using GoogleApi.Entities.Places.Common;
using Microsoft.AspNetCore.Mvc;
using NewHorizon.Models;
using NewHorizon.Models.ColleagueCastleModels;
using NewHorizon.Services.ColleagueCastleServices.Interfaces;
using NewHorizon.Services.Interfaces;
using SkipTrafficLib.Services.Interfaces;

namespace NewHorizon.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlaceSuggestionController : ControllerBase
{
    private readonly ILogger<TrafficDurationController> _logger;
    private readonly IGooglePlaceService googlePlaceService;
    private readonly ISessionTokenManager sessionTokenManager;

    public PlaceSuggestionController(ILogger<TrafficDurationController> logger, IGooglePlaceService googlePlaceService, ISessionTokenManager sessionTokenManager)
    {
        _logger = logger;
        this.googlePlaceService = googlePlaceService;
        this.sessionTokenManager = sessionTokenManager;
    }

    [RequireHttps]
    [ApiKeyRequired]
    [ApiExplorerSettings(GroupName = "v1")]
    [HttpGet(Name = "GetPlaceSugggestion")]
    public async Task<IActionResult> Get(string inputString)
    {
        var apiKey = HttpContext.Request.Headers["X-Api-Key"];
        if (string.IsNullOrEmpty(apiKey))
        {
            return BadRequest("Invalid X-Api-Key");
        }
        bool isValid = await this.sessionTokenManager.ValidateSessionToken(apiKey).ConfigureAwait(false);
        if (!isValid)
        {
            return BadRequest("Invalid X-Api-Key, need session token as X-Api-key");
        }

        var response = await this.googlePlaceService.GetSuggestionsAsync(inputString, apiKey).ConfigureAwait(false);
        return  Ok(response.Select(x => new PlacePredictionApiResponse(x.PlaceId, x.Description)));
    }
}
