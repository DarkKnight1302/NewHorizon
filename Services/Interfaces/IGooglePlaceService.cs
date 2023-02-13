using GoogleApi.Entities.Places.Common;
using GoogleApi.Entities.Places.Details.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SkipTrafficLib.Services.Interfaces
{
    public interface IGooglePlaceService
    {
        /// <summary>
        /// Get suggestions for location based on user input.
        /// </summary>
        /// <param name="input">input string.</param>
        /// <returns>Predictions.</returns>
        public Task<IEnumerable<Prediction>> GetSuggestionsAsync(string input);

        /// <summary>
        /// Get place location.
        /// </summary>
        /// <param name="placeId">PlaceId.</param>
        /// <returns>Result.</returns>
        public Task<DetailsResult> GetPlaceDetailsAsync(string placeId);
    }
}
