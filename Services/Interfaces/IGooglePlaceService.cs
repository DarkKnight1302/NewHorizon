using GoogleApi.Entities.Places.Common;
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
    }
}
