using GoogleApi.Entities.Places.Details.Response;
using NewHorizon.Models.ColleagueCastleModels;
using NewHorizon.Models.ColleagueCastleModels.DatabaseModels;
using NewHorizon.Repositories.Interfaces;
using NewHorizon.Services.ColleagueCastleServices.Interfaces;
using NewHorizon.Utils;
using SkipTrafficLib.Services.Interfaces;

namespace NewHorizon.Services.ColleagueCastleServices
{
    public class SearchPropertyService : ISearchPropertyService
    {
        private readonly IGooglePlaceService googlePlaceService;
        private readonly IPropertyPostRepository propertyPostRepository;
        private readonly IUserRepository userRepository;
        private readonly IPropertyMatchingService propertyMatchingService;

        public SearchPropertyService(
            IGooglePlaceService googlePlaceService,
            IPropertyPostRepository propertyPostRepository,
            IUserRepository userRepository,
            IPropertyMatchingService propertyMatchingService)
        {
            this.googlePlaceService = googlePlaceService;
            this.propertyPostRepository = propertyPostRepository;
            this.userRepository = userRepository;
            this.propertyMatchingService = propertyMatchingService;
        }
        public async Task<(IReadOnlyList<PropertyPostDetails>?, string error)> GetMatchingPropertyListAsync(SearchPropertyRequest searchPropertyRequest)
        {
            User user = await this.userRepository.GetUserByUserNameAsync(searchPropertyRequest.UserId).ConfigureAwait(false);
            if (user == null)
            {
                return (null, "User Not found");
            }
            DetailsResult placeDetails = await this.googlePlaceService.GetPlaceDetailsAsync(searchPropertyRequest.OfficePlaceId);
            if (placeDetails == null)
            {
                return (null, "Incorrect PlaceId");
            }
            string cityName = CityNameUtil.GetCityNameFromPlaceDetails(placeDetails);
            if (cityName == null)
            {
                return (null, "Please provide Specific Location");
            }
            IEnumerable<PropertyPostDetails> propertyPostDetailsList = await this.propertyPostRepository.GetAllPropertyPostDetailsAsync(cityName, user.Company).ConfigureAwait(false);

            List<PropertyPostDetails> matchedProperties = new List<PropertyPostDetails>();
            foreach (var propertyPostDetail in propertyPostDetailsList)
            {
                bool isMatch = await this.propertyMatchingService.IsMatch(propertyPostDetail, searchPropertyRequest).ConfigureAwait(false);
                if (isMatch)
                {
                    matchedProperties.Add(propertyPostDetail);
                }
            }
            return (matchedProperties, string.Empty);
        }
    }
}
