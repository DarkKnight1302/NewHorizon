using Microsoft.Azure.Cosmos;
using MimeKit;
using NewHorizon.Models.ColleagueCastleModels;
using NewHorizon.Services.Interfaces;
using MailKit.Net.Smtp;
using NewHorizon.Services.ColleagueCastleServices.Interfaces;

namespace NewHorizon.Services.ColleagueCastleServices
{
    public class OtpService : IOtpService
    {
        private readonly Container container;
        private readonly Random random;
        private readonly ISecretService secretService;

        public OtpService(ICosmosDbService cosmosDbService, ISecretService secretService)
        {
            this.secretService = secretService;
            container = cosmosDbService.GetContainerFromColleagueCastle("Otp");
            random = new Random();
        }

        public async Task GenerateAndSendOtpAsync(string emailAddress)
        {
            int otp = await GenerateOtpAsync(emailAddress).ConfigureAwait(false);
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Colleague Castle", "admin@colleaguecastle.in"));
            message.To.Add(new MailboxAddress("Fellow Colleague", emailAddress));
            message.Subject = "OTP from ColleagueCastle.in";
            message.Body = new TextPart("plain")
            {
                Text = $"OTP for verification : {otp}"
            };

            using (SmtpClient smtpClient = new SmtpClient())
            {
                smtpClient.Connect("smtp.gmail.com", 465, true);

                //SMTP server authentication if needed
                smtpClient.Authenticate("admin@colleaguecastle.in", secretService.GetSecretValue("COLLEAGUE_CASTLE_EMAIL_PASSWORD"));

                smtpClient.Send(message);

                smtpClient.Disconnect(true);
            }
        }

        public async Task<bool> IsOtpValidAsync(string emailAddress, int otp)
        {
            ItemResponse<OtpObject> itemResponse;
            try
            {
                itemResponse = await container.ReadItemAsync<OtpObject>(emailAddress.ToLower(), new PartitionKey(emailAddress.ToLower())).ConfigureAwait(false);
            }
            catch (CosmosException)
            {
                itemResponse = null;
            }
            if (itemResponse != null && itemResponse.Resource != null)
            {
                if (itemResponse.Resource.Otp == otp && itemResponse.Resource.Expiry >= DateTime.UtcNow)
                {
                    _ = Task.Run(async () => await container.DeleteItemAsync<OtpObject>(emailAddress.ToLower(), new PartitionKey(emailAddress.ToLower())).ConfigureAwait(false));
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
                Otp = random.Next(1000, 9999),
                Expiry = DateTime.UtcNow.AddMinutes(5),
            };
            ItemResponse<OtpObject> itemResponse = await container.UpsertItemAsync(otpObject);
            return itemResponse.Resource.Otp;
        }
    }
}
