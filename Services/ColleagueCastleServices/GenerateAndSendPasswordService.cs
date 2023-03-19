using GoogleApi.Entities.Maps.Common;
using GoogleApi.Entities.Places.Details.Response;
using Microsoft.Extensions.Caching.Memory;
using NewHorizon.Models.ColleagueCastleModels.DatabaseModels;
using NewHorizon.Repositories.Interfaces;
using NewHorizon.Services.ColleagueCastleServices.Interfaces;
using NewHorizon.Services.Interfaces;
using System.Text;

namespace NewHorizon.Services.ColleagueCastleServices
{
    public class GenerateAndSendPasswordService : IGenerateAndSendPasswordService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMemoryCache memoryCache;
        private readonly IMailingService _mailingService;
        private SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

        public GenerateAndSendPasswordService(IUserRepository userRepository, IMemoryCache memoryCache, IMailingService mailingService)
        {
            this._userRepository = userRepository;
            this.memoryCache = memoryCache;
            this._mailingService = mailingService;
        }

        public async Task<bool> GenerateAndSendPassword(string userId)
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                if (this.memoryCache.TryGetValue(userId, out _))
                {
                    return false;
                }
                User user = await this._userRepository.GetUserByUserNameAsync(userId).ConfigureAwait(false);
                if (user == null)
                {
                    return false;
                }
                string password = this.GeneratePassword();
                bool success = await this._userRepository.UpdateUserPassword(user, password).ConfigureAwait(false);
                if (!success)
                {
                    return false;
                }
                string subject = "[ColleagueCastle.in] New Temporary Password";
                string message = $"<!DOCTYPE html>\r\n<html>\r\n  <head>\r\n    <meta charset=\"UTF-8\">\r\n    <title>Password Reset</title>\r\n    <style>\r\n      body {{\r\n        font-family: Arial, sans-serif;\r\n        color: #333333;\r\n        background-color: #f2f2f2;\r\n        margin: 0;\r\n        padding: 0;\r\n      }}\r\n      #wrapper {{\r\n        max-width: 600px;\r\n        margin: 0 auto;\r\n        background-color: #ffffff;\r\n        border-radius: 5px;\r\n        padding: 20px;\r\n        box-shadow: 0 3px 6px rgba(0, 0, 0, 0.1);\r\n      }}\r\n      #header {{\r\n        text-align: center;\r\n        margin-bottom: 20px;\r\n      }}\r\n      #header img {{\r\n        max-width: 200px;\r\n      }}\r\n      h1 {{\r\n        font-size: 24px;\r\n        margin-bottom: 20px;\r\n      }}\r\n      p {{\r\n        font-size: 16px;\r\n        line-height: 1.5;\r\n        margin-bottom: 20px;\r\n      }}\r\n      strong {{\r\n        font-weight: bold;\r\n      }}\r\n      #footer {{\r\n        text-align: center;\r\n        margin-top: 20px;\r\n      }}\r\n    </style>\r\n  </head>\r\n  <body>\r\n    <div id=\"wrapper\">\r\n      <div id=\"header\">\r\n        <img src=\"https://newhorizonblobstorage.blob.core.windows.net/colleaguecastleblob/33bc55de-6a10-414b-8e9d-3d795cec6001.png\" alt=\"Colleague Castle\" width=\"200\" height=\"200\" style=\"display: block; margin: 0 auto;\">\r\n      </div>\r\n      <h1>Password Reset</h1>\r\n      <p>Hi {user.Name},</p>\r\n  Please use the temporary password below to login to your account:</p>\r\n      <p><strong>Temporary Password:</strong> {password}</p>\r\n <p>We recommend that you change your password immediately after logging in with the temporary password to ensure the security of your account.</p>\r\n <div id=\"footer\">\r\nColleague Castle</p>\r\n      </div>\r\n    </div>\r\n  </body>\r\n</html>\r\n";
                this._mailingService.SendMail(user.Email, subject, message, true);
                var memoryCacheEntryOptions = new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) };
                this.memoryCache.Set<bool>(userId, true, memoryCacheEntryOptions);
                return true;
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        private string GeneratePassword(int length = 10)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789@#$%&*";
            var random = new Random();
            var password = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                password.Append(chars[random.Next(chars.Length)]);
            }
            return password.ToString();
        }
    }
}
