using Microsoft.Azure.Cosmos;
using NewHorizon.Models.ColleagueCastleModels;
using NewHorizon.Services.ColleagueCastleServices.Interfaces;
using NewHorizon.Services.Interfaces;
using static System.Net.WebRequestMethods;

namespace NewHorizon.Services.ColleagueCastleServices
{
    public class SignUpTokenService : ISignUpTokenService
    {
        private readonly Container container;

        public SignUpTokenService(ICosmosDbService cosmosDbService)
        {
            this.container = cosmosDbService.GetContainerFromColleagueCastle("SignUpToken");
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

            ItemResponse<SignUpToken> response = await this.container.UpsertItemAsync(signUpToken).ConfigureAwait(false);
            return response.Resource.Token;
        }

        public async Task<bool> VerifySignUpTokenAsync(string token, string emailAddress)
        {
            ItemResponse<SignUpToken> itemResponse;
            try
            {
                itemResponse = await this.container.DeleteItemAsync<SignUpToken>(token, new PartitionKey(token)).ConfigureAwait(false);
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
