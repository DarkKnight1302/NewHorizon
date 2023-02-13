using GoogleApi;
using GoogleApi.Entities.Places.Common;
using GoogleApi.Entities.Places.Details.Request;
using GoogleApi.Entities.Places.Details.Response;
using GoogleApi.Entities.Places.QueryAutoComplete.Request;
using GoogleApi.Entities.Places.QueryAutoComplete.Response;
using GoogleApi.Interfaces.Places;
using Microsoft.Extensions.Caching.Memory;
using NewHorizon.Helpers;
using NewHorizon.Models.ColleagueCastleModels;
using NewHorizon.Services.Interfaces;
using SkipTrafficLib.Services.Interfaces;

namespace SkipTrafficLib.Services
{
    public class GooglePlaceService : IGooglePlaceService
    {
        private readonly IMemoryCache memoryCache;
        private readonly IQueryAutoCompleteApi queryAutoCompleteApi;
        private RequestThresholdPerDay requestThresholdPerDay;
        private readonly ISecretService secretService;

        public GooglePlaceService(ISecretService secretService, IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
            this.queryAutoCompleteApi = GooglePlaces.QueryAutoComplete;
            this.secretService = secretService;
            this.requestThresholdPerDay = new RequestThresholdPerDay(500);
        }

        public async Task<IEnumerable<Prediction>> GetSuggestionsAsync(string input)
        {
            if (!this.requestThresholdPerDay.AllowRequest())
            {
                return Enumerable.Empty<Prediction>();
            }
            var request = new PlacesQueryAutoCompleteRequest { Input = input, Key = this.secretService.GetSecretValue("GOOGLE_PLACE_API_KEY") };
            PlacesQueryAutoCompleteResponse response = await this.queryAutoCompleteApi.QueryAsync(request, null).ConfigureAwait(false);
            return response.Predictions;
        }

        public async Task<DetailsResult> GetPlaceDetailsAsync(string placeId)
        {
            if (this.memoryCache.TryGetValue(placeId, out DetailsResult result))
            {
                return result;
            }
            if (!this.requestThresholdPerDay.AllowRequest())
            {
                return null;
            }

            var request = new PlacesDetailsRequest { PlaceId = placeId, Key = this.secretService.GetSecretValue("GOOGLE_PLACE_API_KEY") };
            PlacesDetailsResponse response = await GooglePlaces.Details.QueryAsync(request, null).ConfigureAwait(false);
            if (response?.Result != null && response.Status == GoogleApi.Entities.Common.Enums.Status.Ok)
            {
                var memoryCacheEntryOptions = new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24) };
                this.memoryCache.Set<DetailsResult>(placeId, response.Result, memoryCacheEntryOptions);
                return response.Result;
            }
            return null;
        }
    }
}
