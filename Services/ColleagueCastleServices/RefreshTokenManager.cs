using NewHorizon.Models.ColleagueCastleModels.DatabaseModels;
using NewHorizon.Services.ColleagueCastleServices.Interfaces;
using NewHorizon.Services.Interfaces;

namespace NewHorizon.Services.ColleagueCastleServices
{
    public class RefreshTokenManager : IRefreshTokenManager
    {
        private ISessionTokenManager _sessionTokenManager;
        private readonly Microsoft.Azure.Cosmos.Container _container;

        public RefreshTokenManager(ISessionTokenManager sessionTokenManager, ICosmosDbService cosmosDbService)
        {
            _sessionTokenManager = sessionTokenManager;
            _container = cosmosDbService.GetContainerFromColleagueCastle("RefreshToken");

        }

        public async Task<string> GenerateRefreshTokenAsync(string userId)
        {
            await this._container.DeleteItemAsync<RefreshToken>(userId, new Microsoft.Azure.Cosmos.PartitionKey(userId)).ConfigureAwait(false);
            var token = new RefreshToken()
            {
                Id = userId,
                UserId = userId,
                Expiry = DateTime.UtcNow.AddDays(30),
                Token = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow,
            };
            var refreshToken = await this._container.CreateItemAsync<RefreshToken>(token).ConfigureAwait(false);

            return refreshToken.Resource.Token;
        }

        public Task<string> GenerateSessionTokenFromRefreshToken(string refreshToken)
        {
            throw new NotImplementedException();
        }
    }
}
