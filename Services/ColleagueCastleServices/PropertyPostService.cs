using GoogleApi.Entities.Common.Enums;
using GoogleApi.Entities.Places.Details.Response;
using NewHorizon.Models.ColleagueCastleModels;
using NewHorizon.Repositories.Interfaces;
using NewHorizon.Services.ColleagueCastleServices.Interfaces;
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

        public async Task<string> CreatePropertyPostAsync(string sessionId, string placeId, string title, string description, List<string> images)
        {
            DetailsResult details = await this.googlePlaceService.GetPlaceDetailsAsync(placeId).ConfigureAwait(false);
            if (details == null)
            {
                return null;
            }
            string userName = await this.sessionTokenManager.GetUserNameFromToken(sessionId);
            if (userName == null)
            {
                return null;
            }
            User user = await this.userRepository.GetUserByUserNameAsync(userName).ConfigureAwait(false);
            if (user == null)
            {
                return null;
            }

            string cityName = null;

            foreach (var addressComponent in details.AddressComponents)
            {
                foreach (var componentType in addressComponent.Types)
                {
                    if (AddressComponentType.Locality == componentType)
                    {
                        cityName = addressComponent.LongName;
                        break;
                    }
                }
            }
            if (string.IsNullOrEmpty(cityName))
            {
                return null;
            }
            Location location = new Location(details.Geometry.Location.Latitude, details.Geometry.Location.Longitude);
            var createPropertyPost = new CreatePropertyObject
            {
                username = userName,
                city = cityName,
                company = user.Company,
                placeId = placeId,
                title = title,
                description = description,
                Images = images,
                FormattedAddress = details.FormattedAddress,
                location = location,
                MapUrl = details.Url,
            };
            string propertyPostId =  await this.propertyPostRepository.CreatePropertyPostAsync(createPropertyPost).ConfigureAwait(false);
            return propertyPostId;
        }

        public Task<bool> DeletePropertyPostAsync(string propertyPostId)
        {
            throw new NotImplementedException();
        }

        public Task<PropertyPost> GetPropertyPostAsync(string propertyPostId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdatePropertyPostAsync(PropertyPost propertyPost)
        {
            throw new NotImplementedException();
        }
    }
}
