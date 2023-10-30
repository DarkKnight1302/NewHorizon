using Microsoft.Azure.Cosmos;
using NewHorizon.Models.ColleagueCastleModels;
using NewHorizon.Models.ColleagueCastleModels.DatabaseModels;
using NewHorizon.Repositories.Interfaces;
using NewHorizon.Services.ColleagueCastleServices.Interfaces;
using NewHorizon.Helpers;

namespace NewHorizon.Services.ColleagueCastleServices
{
    public class InterestService : IInterestService
    {
        private readonly IPropertyPostService propertyPostService;
        private readonly IMailingService mailingService;
        private readonly IUserRepository userRepository;
        private readonly IUserInterestRepository userInterestRepository;
        private readonly IShortListedPropertyRepository shortListedPropertyRepository;
        private readonly string Subject;
        private string Body;

        public InterestService(IPropertyPostService propertyPostService, IMailingService mailingService, IUserRepository userRepository, IUserInterestRepository userInterestRepository, IShortListedPropertyRepository shortListedPropertyRepository)
        {
            this.propertyPostService = propertyPostService;
            this.userInterestRepository = userInterestRepository;
            this.userRepository = userRepository;
            this.mailingService = mailingService;
            this.shortListedPropertyRepository = shortListedPropertyRepository;
            this.Subject = "[ColleagueCastle.in] Interest Shown in your Posted Property";
            this.Body = "<!DOCTYPE html>\r\n<html>\r\n<head>\r\n\t<title>ColleagueCastle Notification: New User Interested in Your Property</title>\r\n\t<meta charset=\"UTF-8\">\r\n\t<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">\r\n\t<style>\r\n\t\tbody {\r\n\t\t\tfont-family: Arial, sans-serif;\r\n\t\t\tfont-size: 16px;\r\n\t\t\tline-height: 1.5;\r\n\t\t\tcolor: #333333;\r\n\t\t\tbackground-color: #f5f5f5;\r\n\t\t\tpadding: 0;\r\n\t\t\tmargin: 0;\r\n\t\t}\r\n\r\n\t\t.container {\r\n\t\t\tmax-width: 600px;\r\n\t\t\tmargin: 0 auto;\r\n\t\t\tpadding: 20px;\r\n\t\t\tbackground-color: #ffffff;\r\n\t\t\tborder-radius: 10px;\r\n\t\t\tbox-shadow: 0 0 10px rgba(0,0,0,0.1);\r\n\t\t}\r\n\r\n\t\th1 {\r\n\t\t\tmargin-top: 0;\r\n\t\t\tfont-size: 28px;\r\n\t\t\tfont-weight: 700;\r\n\t\t\ttext-align: center;\r\n\t\t\tcolor: #333333;\r\n\t\t}\r\n\r\n\t\tp {\r\n\t\t\tmargin: 0 0 20px 0;\r\n\t\t\tfont-size: 16px;\r\n\t\t\tline-height: 1.5;\r\n\t\t\tcolor: #333333;\r\n\t\t}\r\n\r\n\t\t.button {\r\n\t\t\tdisplay: inline-block;\r\n\t\t\tpadding: 10px 20px;\r\n\t\t\tfont-size: 16px;\r\n\t\t\tfont-weight: 700;\r\n\t\t\ttext-align: center;\r\n\t\t\tcolor: #ffffff;\r\n\t\t\tbackground-color: #4caf50;\r\n\t\t\tborder-radius: 5px;\r\n\t\t\ttext-decoration: none;\r\n\t\t}\r\n\r\n\t\t.button:hover {\r\n\t\t\tbackground-color: #388e3c;\r\n\t\t}\r\n\r\n\t\t.footer {\r\n\t\t\tmargin-top: 30px;\r\n\t\t\ttext-align: center;\r\n\t\t\tfont-size: 14px;\r\n\t\t\tline-height: 1.5;\r\n\t\t\tcolor: #999999;\r\n\t\t}\r\n\r\n\t\t.footer a {\r\n\t\t\tcolor: #4caf50;\r\n\t\t\ttext-decoration: none;\r\n\t\t}\r\n\t</style>\r\n</head>\r\n<body>\r\n\t<div class=\"container\">\r\n\t\t<div class=\"header\">\r\n\t\t\t<img src=\"https://newhorizonblobstorage.blob.core.windows.net/colleaguecastleblob/33bc55de-6a10-414b-8e9d-3d795cec6001.png\" alt=\"Colleague Castle\" width=\"200\" height=\"200\" style=\"display: block; margin: 0 auto;\">\r\n\t\t</div>\r\n\t\t<h1>New User Interested in Your Property</h1>\r\n\t\t<p>Hello #postcreator#,</p>\r\n\t\t<p>We wanted to let you know that a user has shown interest in your property listing on ColleagueCastle.in. They are interested in knowing more about the property and possibly becoming your flatmate or Tenant.</p>\r\n\t\t<p>Please log in to your account on ColleagueCastle.in to check the details of the user. </p>\r\n\t\t<p>Thank you for using ColleagueCastle.in to find your perfect flatmate.</p>\r\n\t\t<p>Sincerely,</p>\r\n\t\t<p>The ColleagueCastle.in Team</p>\r\n\t\t<a href=\"https://www.colleaguecastle.in\" class=\"button\">Log In to Your Account</a>\r\n\t\t\n";
        }

        public async Task<IEnumerable<PropertyPostResponse>> FindShortlistedProperties(string UserId)
        {
            List<string> propertyIds = await this.shortListedPropertyRepository.GetShortlistedPropertiesByUser(UserId).ConfigureAwait(false);
            if (propertyIds == null || propertyIds.Count == 0)
            {
                return Enumerable.Empty<PropertyPostResponse>();
            }

            IEnumerable<PropertyPostDetails> properties = await this.propertyPostService.GetPropertyPostsAsync(propertyIds).ConfigureAwait(false);
            if (properties == null || !properties.Any())
            {
                return Enumerable.Empty<PropertyPostResponse>();
            }
            return BaseUtil.ConvertPropertyDetails(properties);
        }

        public async void ShowInterestInPost(string postId, string UserId)
        {
            if (string.IsNullOrWhiteSpace(postId) || string.IsNullOrEmpty(UserId))
            {
                return;
            }
            await this.shortListedPropertyRepository.ShortlistProperty(postId, UserId);
            PropertyPostDetails propertyPost = await this.propertyPostService.GetPropertyPostAsync(postId).ConfigureAwait(false);
            if (propertyPost != null)
            {
                string postCreator = propertyPost.CreatorUserName;
                Models.ColleagueCastleModels.DatabaseModels.User postCreatorUser = await this.userRepository.GetUserByUserNameAsync(postCreator).ConfigureAwait(false);
                if (postCreatorUser != null)
                {
                    bool interestAdded = await this.userInterestRepository.AddInterestForPostAsync(postId: postId, userId: UserId).ConfigureAwait(false);
                    if (interestAdded)
                    {
                        this.Body = this.Body.Replace("#postcreator#", postCreatorUser.Name);
                        this.mailingService.SendMail(postCreatorUser.Email, this.Subject, this.Body, true);
                    }
                }
            }
        }
    }
}
