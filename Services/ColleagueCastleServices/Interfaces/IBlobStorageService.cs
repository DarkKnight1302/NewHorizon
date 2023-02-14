namespace NewHorizon.Services.ColleagueCastleServices.Interfaces
{
    public interface IBlobStorageService
    {
        public Task<string> GenerateBlobStorageKey();
    }
}
