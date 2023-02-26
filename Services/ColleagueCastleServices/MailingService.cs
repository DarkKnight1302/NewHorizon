using MailKit.Net.Smtp;
using MimeKit;
using NewHorizon.Services.ColleagueCastleServices.Interfaces;
using NewHorizon.Services.Interfaces;

namespace NewHorizon.Services.ColleagueCastleServices
{
    public class MailingService : IMailingService
    {
        private readonly ISecretService secretService;

        public MailingService(ISecretService secretService)
        {
            this.secretService = secretService;
        }

        public void SendMail(string emailAddress, string subject, string body, bool isBodyHtml = false)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Colleague Castle", "admin@colleaguecastle.in"));
            message.To.Add(new MailboxAddress("Fellow Colleague", emailAddress));
            message.Subject = subject;
            message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = body
            };
            using (SmtpClient smtpClient = new SmtpClient())
            {
                smtpClient.Connect("smtp.gmail.com", 587, false);

                //SMTP server authentication if needed
                smtpClient.Authenticate("admin@colleaguecastle.in", secretService.GetSecretValue("COLLEAGUE_CASTLE_EMAIL_PASSWORD"));

                smtpClient.Send(message);

                smtpClient.Disconnect(true);
            }
        }
    }
}
