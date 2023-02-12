using Microsoft.Azure.Cosmos;
using NewHorizon.Services.Interfaces;
using System.Net.Mail;
using System.Text;

namespace NewHorizon.Services.ColleagueCastleServices
{
    public class OptService : IOtpService
    {
        private readonly SmtpClient smtpClient;
        private readonly Container container;

        public OptService(ICosmosDbService cosmosDbService)
        {
            this.smtpClient = new SmtpClient();
            this.container = cosmosDbService.GetContainerFromColleagueCastle("Otp");
        }

        public void SendOtp(string emailAddress, int otp)
        {
            MailMessage mail = new MailMessage();
            mail.To.Add(emailAddress);
            mail.From = new MailAddress("no-reply@ColleagueCastle.com");
            mail.Subject = "OTP for Verification by ColleagueCastle.in";
            mail.Body = "Your OTP for verification is: " + otp;
            mail.BodyEncoding = Encoding.UTF8;
            mail.IsBodyHtml = true;
            this.smtpClient.Send(mail);
        }

        private async Task GenerateOtp(string emailAddress)
        {

        }
    }
}
