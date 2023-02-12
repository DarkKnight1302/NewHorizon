namespace NewHorizon.Services.ColleagueCastleServices.Interfaces
{
    public interface IOtpService
    {
        public Task GenerateAndSendOtpAsync(string emailAddress);

        public Task<bool> IsOtpValidAsync(string emailAddress, int otp);
    }
}
