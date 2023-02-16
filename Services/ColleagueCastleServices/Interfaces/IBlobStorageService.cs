namespace NewHorizon.Services.ColleagueCastleServices.Interfaces
{
    public interface IBlobStorageService
    {
        public string GenerateBlobStorageAccessToken();

        public Task<string> UploadFileToBlobAsync(IFormFile file, string fileName);
    }
}
