using Azure.Core;
using GoogleApi.Entities.Places.Details.Response;
using NewHorizon.Repositories.Interfaces;
using NewHorizon.Services.ColleagueCastleServices.Interfaces;
using SkipTrafficLib.Services.Interfaces;

namespace NewHorizon.Repositories
{
    public class PropertyPostRepository : IPropertyPostRepository
    {
        private readonly IGooglePlaceService GooglePlaceService;
        private readonly ISessionTokenManager sessionTokenManager;

        public PropertyPostRepository(IGooglePlaceService googlePlaceService, ISessionTokenManager sessionTokenManager) 
        {
            this.GooglePlaceService = googlePlaceService;
            this.sessionTokenManager = sessionTokenManager;
        }

        public async Task<bool> CreatePropertyPostAsync(string sessionId, string placeId, string title, string description, List<string> images)
        {
            DetailsResult details = await this.GooglePlaceService.GetPlaceDetailsAsync(placeId).ConfigureAwait(false);
            if (details == null)
            {
                return false;
            }
            string userName = await this.sessionTokenManager.GetUserNameFromToken(sessionId);
            if (userName == null)
            {
                return false;
            }
            return true;
        }
    }
}
