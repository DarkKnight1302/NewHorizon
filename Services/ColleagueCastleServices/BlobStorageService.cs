using Azure.Storage.Blobs;
using NewHorizon.Services.ColleagueCastleServices.Interfaces;

namespace NewHorizon.Services.ColleagueCastleServices
{
    public class BlobStorageService : IBlobStorageService
    {
        public Task<string> GenerateBlobStorageKey()
        {
            var blobServiceClient = new BlobServiceClient("YOUR_CONNECTION_STRING");
            return null;
        }
    }
}
