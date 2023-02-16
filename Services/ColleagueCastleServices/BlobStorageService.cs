using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NewHorizon.Services.ColleagueCastleServices.Interfaces;
using NewHorizon.Services.Interfaces;
using System.Reflection.Metadata;

namespace NewHorizon.Services.ColleagueCastleServices
{
    public class BlobStorageService : IBlobStorageService
    {
        private ISecretService secretService;
        private readonly BlobServiceClient BlobServiceClient;
        private readonly BlobContainerClient containerClient;

        public BlobStorageService(ISecretService secretService)
        {
            this.secretService = secretService;
            this.BlobServiceClient = new BlobServiceClient(secretService.GetSecretValue("BLOB_STORAGE_CONNECTION_STRING"));
            this.containerClient = this.BlobServiceClient.GetBlobContainerClient("colleaguecastleblob");
        }

        public string GenerateBlobStorageAccessToken()
        {

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
            string sasToken = sasBuilder.ToSasQueryParameters(new StorageSharedKeyCredential(this.BlobServiceClient.AccountName, secretService.GetSecretValue("BLOB_STORAGE_ACCOUNT_KEY"))).ToString();

            return sasToken;
        }

        public async Task<string> UploadFileToBlobAsync(IFormFile file, string fileName)
        {
            string blobName = Guid.NewGuid().ToString() + Path.GetExtension(fileName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            using (Stream fileStream = file.OpenReadStream())
            {
                var blobInfo = await blobClient.UploadAsync(fileStream, new BlobUploadOptions
                {
                    HttpHeaders = new BlobHttpHeaders
                    {
                        ContentType = GetContentType(Path.GetExtension(fileName))
                    }
                });
            }

            return blobClient.Uri.AbsoluteUri;

        }
        private string GetContentType(string fileExtension)
        {
            if (string.IsNullOrEmpty(fileExtension))
            {
                throw new ArgumentNullException(nameof(fileExtension));
            }

            if (!fileExtension.StartsWith("."))
            {
                fileExtension = "." + fileExtension;
            }

            var provider = new FileExtensionContentTypeProvider();

            if (provider.TryGetContentType(fileExtension, out string contentType))
            {
                return contentType;
            }

            return "application/octet-stream";
        }

    }
}
