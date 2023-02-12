using Microsoft.Azure.Cosmos;
using NewHorizon.Models.ColleagueCastleModels;
using NewHorizon.Services.ColleagueCastleServices.Interfaces;
using NewHorizon.Services.Interfaces;
using System.Net.Mail;
using System.Text;

namespace NewHorizon.Services.ColleagueCastleServices
{
    public class OtpService : IOtpService
    {
        private readonly SmtpClient smtpClient;
        private readonly Container container;
        private readonly Random random;

        public OtpService(ICosmosDbService cosmosDbService)
        {
            this.smtpClient = new SmtpClient();
            this.container = cosmosDbService.GetContainerFromColleagueCastle("Otp");
            this.random = new Random();
        }

        public async Task GenerateAndSendOtpAsync(string emailAddress)
        {
            int otp = await this.GenerateOtpAsync(emailAddress).ConfigureAwait(false);
            MailMessage mail = new MailMessage();
            mail.To.Add(emailAddress);
            mail.From = new MailAddress("no-reply@ColleagueCastle.com");
            mail.Subject = "OTP for Verification by ColleagueCastle.in";
            mail.Body = "Your OTP for verification is: " + otp;
            mail.BodyEncoding = Encoding.UTF8;
            mail.IsBodyHtml = true;
            this.smtpClient.Send(mail);
        }

        public async Task<bool> IsOtpValidAsync(string emailAddress, int otp)
        {
            ItemResponse<OtpObject> itemResponse;
            try
            {
                itemResponse = await this.container.DeleteItemAsync<OtpObject>(emailAddress.ToLower(), new PartitionKey(emailAddress.ToLower())).ConfigureAwait(false);
            }
            catch(CosmosException)
            {
                itemResponse = null;
            }
            if (itemResponse != null && itemResponse.Resource != null)
            {
                if (itemResponse.Resource.Otp == otp && itemResponse.Resource.Expiry >= DateTime.UtcNow) 
                { 
                    return true;
                }
            }

            return false;
        }

        private async Task<int> GenerateOtpAsync(string emailAddress)
        {
            OtpObject otpObject = new OtpObject
            {
                Id = emailAddress.ToLower(),
                Uid = emailAddress.ToLower(),
                Otp = this.random.Next(1000, 9999),
                Expiry = DateTime.UtcNow.AddMinutes(5),
            };
            ItemResponse<OtpObject> itemResponse = await this.container.UpsertItemAsync(otpObject);
            return itemResponse.Resource.Otp;
        }
    }
}
