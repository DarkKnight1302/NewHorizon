using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NewHorizon.Services.ColleagueCastleServices.Interfaces;
using NewHorizon.Services.Interfaces;

namespace NewHorizon.Services.ColleagueCastleServices
{
    public class BlobStorageService : IBlobStorageService
    {
        private ISecretService secretService;

        public BlobStorageService(ISecretService secretService)
        {
            this.secretService = secretService;
        }

        public string GenerateBlobStorageAccessToken()
        {
            var blobServiceClient = new BlobServiceClient(secretService.GetSecretValue("BLOB_STORAGE_CONNECTION_STRING"));
            DateTimeOffset expiryTime = DateTimeOffset.UtcNow.AddHours(1); // the token will expire in 1 hour

            // Create a BlobSasBuilder object to define the permissions and other parameters for the SAS token
            BlobSasBuilder sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = "colleaguecastleblob",
                ExpiresOn = expiryTime,
                Protocol = SasProtocol.Https,
                StartsOn = DateTimeOffset.UtcNow
            };
            sasBuilder.SetPermissions(BlobContainerSasPermissions.Read | BlobContainerSasPermissions.Add);

            // Generate the SAS token for the container
            string sasToken = sasBuilder.ToSasQueryParameters(new StorageSharedKeyCredential(blobServiceClient.AccountName, secretService.GetSecretValue("BLOB_STORAGE_ACCOUNT_KEY"))).ToString();

            return sasToken;
        }
    }
}
