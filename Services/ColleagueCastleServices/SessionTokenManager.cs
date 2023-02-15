using Microsoft.Azure.Cosmos;
using NewHorizon.Models;
using NewHorizon.Models.ColleagueCastleModels;
using NewHorizon.Services.ColleagueCastleServices.Interfaces;
using NewHorizon.Services.Interfaces;
using System.ComponentModel;

namespace NewHorizon.Services.ColleagueCastleServices
{
    public class SessionTokenManager : ISessionTokenManager
    {
        private readonly Microsoft.Azure.Cosmos.Container _container;

        public SessionTokenManager(ICosmosDbService cosmosDbService)
        {
            _container = cosmosDbService.GetContainerFromColleagueCastle("UserSessionToken");
        }

        public async Task<string> GenerateSessionToken(string userId)
        {
            string existingToken = await GetTokenIfAlreadySignedIn(userId).ConfigureAwait(false);
            if (existingToken != null)
            {
                return existingToken;
            }
            string sessionToken = Guid.NewGuid().ToString();
            DateTime expiry = DateTime.UtcNow.AddMinutes(30);

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

        public async Task<string> GetUserNameFromToken(string sessionToken)
        {
            QueryDefinition queryDefinition = new QueryDefinition($"SELECT * FROM c WHERE c.Token = @value")
            .WithParameter("@value", sessionToken);

            // Execute the query and retrieve the results
            FeedIterator<UserSessionToken> queryResultSet = _container.GetItemQueryIterator<UserSessionToken>(queryDefinition);
            while (queryResultSet.HasMoreResults)
            {
                FeedResponse<UserSessionToken> currentResultSet = await queryResultSet.ReadNextAsync().ConfigureAwait(false);
                if (currentResultSet != null && currentResultSet.Count > 0)
                {
                    return currentResultSet.First().UserId;
                }
            }
            return null;
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
