using GoogleApi.Entities.Common.Enums;
using GoogleApi.Entities.Places.Details.Response;
using NewHorizon.Models.ColleagueCastleModels;
using NewHorizon.Models.ColleagueCastleModels.DatabaseModels;
using NewHorizon.Repositories.Interfaces;
using NewHorizon.Services.ColleagueCastleServices.Interfaces;
using NewHorizon.Utils;
using SkipTrafficLib.Services.Interfaces;

namespace NewHorizon.Services.ColleagueCastleServices
{
    public class PropertyPostService : IPropertyPostService
    {
        private readonly IGooglePlaceService googlePlaceService;
        private readonly IUserRepository userRepository;
        private readonly ISessionTokenManager sessionTokenManager;
        private readonly IPropertyPostRepository propertyPostRepository;

        public PropertyPostService(
            IGooglePlaceService googlePlaceService,
            IUserRepository userRepository,
            ISessionTokenManager sessionTokenManager,
            IPropertyPostRepository propertyPostRepository)
        {
            this.googlePlaceService = googlePlaceService;
            this.userRepository = userRepository;
            this.sessionTokenManager = sessionTokenManager;
            this.propertyPostRepository = propertyPostRepository;
        }

        public async Task<string> CreatePropertyPostAsync(CreatePropertyPostRequest createPropertyPostRequest)
        {
            DetailsResult placeDetails = await googlePlaceService.GetPlaceDetailsAsync(createPropertyPostRequest.PlaceId).ConfigureAwait(false);
            if (placeDetails == null)
            {
                return null;
            }
            string userName = await sessionTokenManager.GetUserNameFromToken(createPropertyPostRequest.SessionId);
            if (userName == null)
            {
                return null;
            }
            User user = await userRepository.GetUserByUserNameAsync(userName).ConfigureAwait(false);
            if (user == null)
            {
                return null;
            }

            string cityName = CityNameUtil.GetCityNameFromPlaceDetails(placeDetails);

            if (string.IsNullOrEmpty(cityName))
            {
                return null;
            }
            Location location = new Location(placeDetails.Geometry.Location.Latitude, placeDetails.Geometry.Location.Longitude);
            var createPropertyPost = new CreatePropertyObject
            {
                username = userName,
                city = cityName,
                company = user.Company,
                placeId = createPropertyPostRequest.PlaceId,
                title = createPropertyPostRequest.Title,
                description = createPropertyPostRequest.Description,
                Images = createPropertyPostRequest.Images,
                FormattedAddress = placeDetails.FormattedAddress,
                location = location,
                MapUrl = placeDetails.Url,
                RentAmount = createPropertyPostRequest.RentAmount,
                PropertyType = createPropertyPostRequest.PropertyType,
                TenantPreference = createPropertyPostRequest.TenantPreference,
                FoodPreference = createPropertyPostRequest.FoodPreference,
                FurnishingType = createPropertyPostRequest.FurnishingType,
                FlatType = createPropertyPostRequest.FlatType,
                Drinking = createPropertyPostRequest.Drinking,
                Smoking = createPropertyPostRequest.Smoking,
                ExperienceRange = createPropertyPostRequest.ExperienceRange,
            };
            string propertyPostId = await propertyPostRepository.CreatePropertyPostAsync(createPropertyPost).ConfigureAwait(false);
            return propertyPostId;
        }

        public async Task<bool> DeletePropertyPostAsync(string propertyPostId, string userId)
        {
            IEnumerable<PropertyPostDetails> userPropertyPosts = await this.GetUserPropertyPostsAsync(userId).ConfigureAwait(false);
            if (userPropertyPosts.Any())
            {
                PropertyPostDetails? propertyPost = userPropertyPosts.Where(post => propertyPostId == post.Id).FirstOrDefault();
                if (propertyPost != null)
                {
                    bool success = await this.propertyPostRepository.DeletePropertyPostAsync(propertyPost);
                    return success;
                }
            }
            return false;
        }

        public async Task<PropertyPostDetails> GetPropertyPostAsync(string propertyPostId)
        {
            return await this.propertyPostRepository.GetPropertryPostDetailsById(propertyPostId).ConfigureAwait(false);
        }

        public async Task<IEnumerable<PropertyPostDetails>> GetUserPropertyPostsAsync(string userId)
        {
            IEnumerable<PropertyPostDetails> propertyPosts = await this.propertyPostRepository.GetAllAvailablePostOfUserAsync(userId);
            return propertyPosts;
        }

        public Task<bool> UpdatePropertyPostAsync(PropertyPost propertyPost)
        {
            throw new NotImplementedException();
        }
    }
}
