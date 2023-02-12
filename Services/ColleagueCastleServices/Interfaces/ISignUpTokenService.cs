namespace NewHorizon.Services.ColleagueCastleServices.Interfaces
{
    public interface ISignUpTokenService
    {
        public Task<string> GenerateSignUpTokenAsync(string emailAddress);

        public Task<bool> VerifySignUpTokenAsync(string token, string emailAddress);
    }
}
