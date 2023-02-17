using Microsoft.Azure.Cosmos;
using NewHorizon.Models;
using NewHorizon.Models.ColleagueCastleModels.DatabaseModels;
using NewHorizon.Services.ColleagueCastleServices.Interfaces;
using NewHorizon.Services.Interfaces;
using static System.Net.WebRequestMethods;

namespace NewHorizon.Services.ColleagueCastleServices
{
    public class SignUpTokenService : ISignUpTokenService, IClearExpiredData
    {
        private readonly Container container;

        public SignUpTokenService(ICosmosDbService cosmosDbService)
        {
            container = cosmosDbService.GetContainerFromColleagueCastle("SignUpToken");
        }

        public async Task ClearData()
        {
            List<string> itemsToDelete = new List<string>();
            var feedIterator = this.container.GetItemQueryIterator<SignUpToken>();
            while (feedIterator.HasMoreResults)
            {
                FeedResponse<SignUpToken> response = await feedIterator.ReadNextAsync().ConfigureAwait(false);
                if (response != null && response.Resource.Any())
                {
                    foreach (var signUpToken in response.Resource)
                    {
                        if (signUpToken.Expiry < DateTime.UtcNow)
                        {
                            itemsToDelete.Add(signUpToken.Id);
                        }
                    }
                }
            }

            if (itemsToDelete.Count > 0)
            {
                foreach (var id in itemsToDelete)
                {
                    await this.container.DeleteItemAsync<SignUpToken>(id, new PartitionKey(id)).ConfigureAwait(false);
                }
            }
        }

        public async Task<string> GenerateSignUpTokenAsync(string emailAddress)
        {
            string token = Guid.NewGuid().ToString();
            SignUpToken signUpToken = new SignUpToken
            {
                Id = token,
                Uid = emailAddress,
                EmailAddress = emailAddress.ToLower(),
                Token = token,
                Expiry = DateTime.UtcNow.AddMinutes(5),
            };

            ItemResponse<SignUpToken> response = await container.UpsertItemAsync(signUpToken).ConfigureAwait(false);
            return response.Resource.Token;
        }

        public async Task<bool> VerifySignUpTokenAsync(string token, string emailAddress)
        {
            ItemResponse<SignUpToken> itemResponse;
            try
            {
                itemResponse = await container.ReadItemAsync<SignUpToken>(token, new PartitionKey(token)).ConfigureAwait(false);
            }
            catch (CosmosException)
            {
                itemResponse = null;
            }
            if (itemResponse != null && itemResponse.Resource != null)
            {
                if (itemResponse.Resource.EmailAddress == emailAddress.ToLower() && itemResponse.Resource.Expiry >= DateTime.UtcNow)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
