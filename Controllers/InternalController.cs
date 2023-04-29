using Microsoft.AspNetCore.Mvc;
using NewHorizon.Models.ColleagueCastleModels;
using NewHorizon.Repositories.Interfaces;
using NewHorizon.Services.ColleagueCastleServices.Interfaces;
using NewHorizon.Services.Interfaces;
using SkipTrafficLib.Services.Interfaces;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace NewHorizon.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InternalController : ControllerBase
    {
        private readonly IUserRepository userRepository;
        private readonly ISecretService secretService;
        private readonly IGooglePlaceService googlePlaceService;
        private readonly IPropertyPostService propertyPostService;
        private readonly ISessionTokenManager sessionTokenManager;
        private readonly Random random;

        public InternalController(
            IUserRepository userRepository,
            ISecretService secretService,
            IGooglePlaceService googlePlaceService,
            IPropertyPostService propertyPostService,
            ISessionTokenManager sessionTokenManager)
        {
            this.userRepository = userRepository;
            this.secretService = secretService;
            this.googlePlaceService = googlePlaceService;
            this.propertyPostService = propertyPostService;
            this.sessionTokenManager = sessionTokenManager;
            this.random = new Random();
        }

        [ApiExplorerSettings(GroupName = "v1")]
        [HttpPost("delete-user")]
        public async Task<IActionResult> DeleteUser([FromBody] DeleteUserRequest request)
        {
            if (request.SecretToken == null)
            {
                return BadRequest("Invalid secret token");
            }

            if (request.SecretToken == this.secretService.GetSecretValue("INTERNAL_SECRET_TOKEN"))
            {
                bool userDeleted = await this.userRepository.DeleteUser(request.UserId).ConfigureAwait(false);
                if (userDeleted)
                {
                    return Ok("User Deleted Successfully");
                }
            }

            return BadRequest("Unable to delete User");
        }

        [ApiExplorerSettings(GroupName = "v1")]
        [HttpPost("get-place-details")]
        public async Task<IActionResult> GetPlaceDetails([FromBody] GetPlaceDetailsRequest request)
        {
            if (request.SecretToken == null)
            {
                return BadRequest("Invalid secret token");
            }

            if (request.SecretToken == this.secretService.GetSecretValue("INTERNAL_SECRET_TOKEN"))
            {
                var details = await this.googlePlaceService.GetPlaceDetailsAsync(request.PlaceId).ConfigureAwait(false);
                return Ok(details);
            }

            return BadRequest("Not able to fetch details.");
        }

        [ApiExplorerSettings(GroupName = "v1")]
        [HttpPost("create-admin-user")]
        public async Task<IActionResult> CreateAdminUser([FromBody] CreateAdminUserRequest request)
        {
            if (request.SecretToken == null)
            {
                return BadRequest("Invalid secret token");
            }

            if (request.SecretToken == this.secretService.GetSecretValue("INTERNAL_SECRET_TOKEN"))
            {
                bool userCreated = await this.userRepository.CreateAdminUser(request.UserName, request.Password).ConfigureAwait(false);
                if (userCreated)
                {
                    return Ok("User Created Successfully");
                }
            }

            return BadRequest("Unable to Create User");
        }

        [ApiKeyRequired]
        [ApiExplorerSettings(GroupName = "v1")]
        [HttpGet("create-sample-properties")]
        public async Task<IActionResult> CreateSampleProperties()
        {
            List<string> placeIds = new List<string> { "ChIJ8bRtEy6TyzsRj4xSSg6OD-c", "ChIJJalC5VbtyzsRWGBN1U5lluw", "ChIJtY8ZERWTyzsRWF4w74xJYR0", "ChIJYw_sufTvDDkRybApcpA1jKs", "ChIJFbC3X1IYDTkRozCEBLvWPWg", "ChIJ4Vadca0VrjsRq0D94abv8t0", "ChIJV_D8ED_lDDkRKSIGsmkkgZA", "ChIJEUdt5i-XyzsRE1OhHGknLX8", "ChIJw1QJcoeRyzsRzf8xaaKUR0w", "ChIJa9jKxW6TyzsR9YpY6_WkRhU", "ChIJF_CDDv0VrjsRZSYEiJYX8QE", "ChIJO6SQa68_rjsRDezet6io9KU" };
            string secretToken = HttpContext.Request.Headers["X-Api-Key"];
            if (secretToken == null || string.IsNullOrEmpty(secretToken))
            {
                return BadRequest("Invalid secret token");
            }
            string sessionToken = await this.sessionTokenManager.GenerateSessionToken("robin2");
            if (secretToken == this.secretService.GetSecretValue("INTERNAL_SECRET_TOKEN"))
            {
                for (int i = 0; i < 10; i++)
                {
                    CreatePropertyPostRequest createPropertyPostRequest = new CreatePropertyPostRequest();
                    createPropertyPostRequest.Title = $"Test_{i}";
                    createPropertyPostRequest.Description = $"Description 123";
                    createPropertyPostRequest.Images = new List<string> { "https://newhorizonblobstorage.blob.core.windows.net/colleaguecastleblob/33bc55de-6a10-414b-8e9d-3d795cec6001.png" };
                    createPropertyPostRequest.SessionId = sessionToken;
                    createPropertyPostRequest.FoodPreference = (FoodPreference)this.random.Next(0, 2);
                    createPropertyPostRequest.ExperienceRange = new ExperienceRange() { MinExpYears = this.random.Next(0, 5), MaxExpYears = this.random.Next(5, 10) };
                    createPropertyPostRequest.PropertyType = (PropertyType)this.random.Next(0, 2);
                    createPropertyPostRequest.FurnishingType = (FurnishingType)this.random.Next(0, 3);
                    createPropertyPostRequest.Drinking = (Drinking)this.random.Next(0, 2);
                    createPropertyPostRequest.FlatType = (FlatType)this.random.Next(0, 6);
                    int randomPlace = this.random.Next(0, placeIds.Count);
                    createPropertyPostRequest.PlaceId = placeIds[randomPlace];
                    createPropertyPostRequest.RentAmount = this.random.Next(5000, 50000);
                    createPropertyPostRequest.Smoking = (Smoking)this.random.Next(0, 2);
                    createPropertyPostRequest.TenantPreference = (TenantPreference)this.random.Next(0, 4);
                    string id = await this.propertyPostService.CreatePropertyPostAsync(createPropertyPostRequest);
                    if (string.IsNullOrEmpty(id))
                    {
                        Debug.WriteLine($"Cannot create User for PlaceId {placeIds[randomPlace]}");
                    }
                }
                return Ok("Successfully created Users");
            }

            return BadRequest("Unable to Create User");
        }
    }
}
