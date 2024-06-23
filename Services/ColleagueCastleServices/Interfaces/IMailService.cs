namespace NewHorizon.Services.ColleagueCastleServices.Interfaces
{
    public interface IMailingService
    {
        public void SendMail(string emailAddress, string subject, string message, bool isBodyHtml = false);

        public void SendGroundMail(string emailAddress, string subject, string message, bool isBodyHtml = false);
    }
}
