using GoogleApi.Entities.Places.Details.Response;
using NewHorizon.Models.ColleagueCastleModels;
using NewHorizon.Models.ColleagueCastleModels.DatabaseModels;
using NewHorizon.Repositories.Interfaces;
using NewHorizon.Services.ColleagueCastleServices.Interfaces;
using NewHorizon.Utils;
using SkipTrafficLib.Services.Interfaces;
using System.Collections.Concurrent;

namespace NewHorizon.Services.ColleagueCastleServices
{
    public class SearchPropertyService : ISearchPropertyService
    {
        private readonly IGooglePlaceService googlePlaceService;
        private readonly IPropertyPostRepository propertyPostRepository;
        private readonly IUserRepository userRepository;
        private readonly IPropertyMatchingService propertyMatchingService;
        private readonly IPropertySortingService propertySortingService;

        public SearchPropertyService(
            IGooglePlaceService googlePlaceService,
            IPropertyPostRepository propertyPostRepository,
            IUserRepository userRepository,
            IPropertyMatchingService propertyMatchingService,
            IPropertySortingService propertySortingService)
        {
            this.googlePlaceService = googlePlaceService;
            this.propertyPostRepository = propertyPostRepository;
            this.userRepository = userRepository;
            this.propertyMatchingService = propertyMatchingService;
            this.propertySortingService = propertySortingService;
        }

        public async Task<(IEnumerable<PropertyPostResponse>?, string error)> GetMatchingPropertyListAsync(SearchPropertyRequest searchPropertyRequest)
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
            Location location = new Location(placeDetails.Geometry.Location.Latitude, placeDetails.Geometry.Location.Longitude);
            IEnumerable<PropertyPostDetails> propertyPostDetailsList = await this.propertyPostRepository.GetAllPropertyPostDetailsAsync(location, user.Company, searchPropertyRequest.SearchRadiusInKm).ConfigureAwait(false);

            ConcurrentBag<PropertyPostDetails> matchedProperties = new ConcurrentBag<PropertyPostDetails>();
            List<Task> matchingTasks = new List<Task>();
            foreach (var propertyPostDetail in propertyPostDetailsList)
            {
                matchingTasks.Add(Task.Run(async () =>
                {
                    bool isMatch = await this.propertyMatchingService.IsMatch(propertyPostDetail, searchPropertyRequest).ConfigureAwait(false);
                    if (isMatch)
                    {
                        matchedProperties.Add(propertyPostDetail);
                    }
                }));
            }
            await Task.WhenAll(matchingTasks).ConfigureAwait(false);
            IEnumerable<PropertyPostResponse> responseProperties = matchedProperties.Select<PropertyPostDetails, PropertyPostResponse>(x => new PropertyPostResponse { Id = x.Id, CreatorUserName = x.CreatorUserName, Description = x.Description, Drinking = x.Drinking, ExperienceRange = x.ExperienceRange, FlatType = x.FlatType, FoodPreference = x.FoodPreference, FormattedAddress = x.FormattedAddress, FurnishingType = x.FurnishingType, Images = x.Images, InterestIds = x.InterestIds, MapUrl = x.MapUrl, PropertyType = x.PropertyType, RentAmount = x.RentAmount, Smoking = x.Smoking, TenantPreference = x.TenantPreference, Title = x.Title, Views = x.Views, Location = x.Location });
            responseProperties = await this.propertySortingService.SortProperties(responseProperties, searchPropertyRequest).ConfigureAwait(false);
            return (responseProperties, string.Empty);
        }
    }
}
