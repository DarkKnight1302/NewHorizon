namespace NewHorizon.Services.Interfaces
{
    public interface IOtpService
    {
        public void SendOtp(string emailAddress, int otp);
    }
}
