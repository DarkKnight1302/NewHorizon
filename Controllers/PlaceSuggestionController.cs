using GoogleApi.Entities.Places.Common;
using Microsoft.AspNetCore.Mvc;
using NewHorizon.Models;
using NewHorizon.Services.Interfaces;
using SkipTrafficLib.Services.Interfaces;

namespace NewHorizon.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlaceSuggestionController : ControllerBase
{
    private readonly ILogger<TrafficDurationController> _logger;
    private readonly IGooglePlaceService googlePlaceService;

    public PlaceSuggestionController(ILogger<TrafficDurationController> logger, IGooglePlaceService googlePlaceService)
    {
        _logger = logger;
        this.googlePlaceService = googlePlaceService;
    }

    [ApiExplorerSettings(GroupName = "v1")]
    [HttpGet(Name = "GetPlaceSugggestion")]
    public async Task<IEnumerable<PlacePrediction>> Get(string inputString) 
    {
        var response = await this.googlePlaceService.GetSuggestionsAsync(inputString).ConfigureAwait(false);
        return response.Select(x => new PlacePrediction(x.PlaceId, x.Description));
    }
}
