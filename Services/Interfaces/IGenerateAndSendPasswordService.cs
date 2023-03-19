namespace NewHorizon.Services.Interfaces
{
    public interface IGenerateAndSendPasswordService
    {
        public Task<bool> GenerateAndSendPassword(string userId);
    }
}
