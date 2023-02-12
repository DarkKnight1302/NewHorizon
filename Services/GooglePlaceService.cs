using GoogleApi;
using GoogleApi.Entities.Places.Common;
using GoogleApi.Entities.Places.QueryAutoComplete.Request;
using GoogleApi.Entities.Places.QueryAutoComplete.Response;
using GoogleApi.Interfaces.Places;
using NewHorizon.Helpers;
using NewHorizon.Services.Interfaces;
using SkipTrafficLib.Services.Interfaces;

namespace SkipTrafficLib.Services
{
    public class GooglePlaceService : IGooglePlaceService
    {
        private readonly IQueryAutoCompleteApi queryAutoCompleteApi;
        private RequestThresholdPerDay requestThresholdPerDay;
        private readonly ISecretService secretService;

        public GooglePlaceService(ISecretService secretService)
        {
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
    }
}
