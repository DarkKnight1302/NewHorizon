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
            
            string secretToken = HttpContext.Request.Headers["X-Api-Key"];
            if (secretToken == null || string.IsNullOrEmpty(secretToken))
            {
                return BadRequest("Invalid secret token");
            }
            List<string> placeIds = new List<string>();
            string sessionToken = await this.sessionTokenManager.GenerateSessionToken("robin2");
            List<string> cities = new List<string>() { "Hyderabad, India", "Bangalore, India", "Delhi, India", "Noida, India", "Mumbai, India", "Pune, India", "Gurugram, India", "Indore, India", "Jaipur, India", "Ahmedabad, India", "Chennai, India" };
            int placeIdCount = 0;
            if (secretToken == this.secretService.GetSecretValue("INTERNAL_SECRET_TOKEN"))
            {
                foreach (var c in cities)
                {
                    var placePredictions = await this.googlePlaceService.GetSuggestionsAsync($"housing Societies in {c}", sessionToken);
                    if (placePredictions != null)
                    {
                        foreach (var placePrediction in placePredictions)
                        {
                            placeIds.Add(placePrediction.PlaceId);
                        }
                    }
                }
                for (int i = 0; i < 500; i++)
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
                    createPropertyPostRequest.PlaceId = placeIds[placeIdCount++];
                    if (placeIdCount >= placeIds.Count)
                    {
                        break;
                    }
                    createPropertyPostRequest.RentAmount = this.random.Next(5000, 50000);
                    createPropertyPostRequest.Smoking = (Smoking)this.random.Next(0, 2);
                    createPropertyPostRequest.TenantPreference = (TenantPreference)this.random.Next(0, 4);
                    string id = await this.propertyPostService.CreatePropertyPostAsync(createPropertyPostRequest);
                    if (string.IsNullOrEmpty(id))
                    {
                        Debug.WriteLine($"Cannot create User for PlaceId {placeIds[placeIdCount]}");
                    }
                }
                return Ok("Successfully created Users");
            }

            return BadRequest("Unable to Create User");
        }
    }
}
