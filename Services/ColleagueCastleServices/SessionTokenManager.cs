using Microsoft.Azure.Cosmos;
using NewHorizon.Models;
using NewHorizon.Models.ColleagueCastleModels;
using NewHorizon.Services.ColleagueCastleServices.Interfaces;
using NewHorizon.Services.Interfaces;

namespace NewHorizon.Services.ColleagueCastleServices
{
    public class SessionTokenManager : ISessionTokenManager
    {
        private readonly Container _container;

        public SessionTokenManager(ICosmosDbService cosmosDbService)
        {
            this._container = cosmosDbService.GetContainerFromColleagueCastle("UserSessionToken");
        }

        public async Task<string> GenerateSessionToken(string userId)
        {
            string existingToken = await this.GetTokenIfAlreadySignedIn(userId).ConfigureAwait(false);
            if (existingToken != null )
            {
                return existingToken;
            }
            string sessionToken = Guid.NewGuid().ToString();
            DateTime expiry = DateTime.UtcNow.AddHours(1);

            var session = new UserSessionToken
            {
                Id = userId,
                UserId = userId,
                Token = sessionToken,
                Expiry = expiry
            };

            await _container.UpsertItemAsync(session).ConfigureAwait(false);

            return sessionToken;
        }

        public async Task<bool> ValidateSessionToken(string userId, string sessionToken)
        {
            ItemResponse<UserSessionToken> responseToken;
            try
            {
                responseToken = await _container.ReadItemAsync<UserSessionToken>(userId, new PartitionKey(userId));
            }
            catch (CosmosException)
            {
                responseToken = null;
            }
            if (responseToken != null && responseToken.Resource.Token == sessionToken)
            {
                return responseToken.Resource.Expiry >= DateTime.UtcNow;
            }

            return false;
        }

        private async Task<string> GetTokenIfAlreadySignedIn(string userId)
        {
            ItemResponse<UserSessionToken> responseToken;
            try
            {
                responseToken = await _container.ReadItemAsync<UserSessionToken>(userId, new PartitionKey(userId));
            }
            catch (CosmosException)
            {
                responseToken = null;
            }
            if (responseToken != null && responseToken.Resource.Expiry >= DateTime.UtcNow)
            {
                return responseToken.Resource.Token;
            }
            return null;
        }
    }
}
