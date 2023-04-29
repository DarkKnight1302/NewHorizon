using NewHorizon.Models.ColleagueCastleModels;
using NewHorizon.Services.ColleagueCastleServices.Interfaces;

namespace NewHorizon.Services.ColleagueCastleServices
{
    public class PropertySortingService : IPropertySortingService
    {
        private readonly IEnumerable<ICriteriaSorting> criteriaSortingServices;

        public PropertySortingService(IServiceProvider serviceProvider)
        {
            this.criteriaSortingServices = serviceProvider.GetServices<ICriteriaSorting>();
        }

        public async Task<List<PropertyPostResponse>> SortProperties(IEnumerable<PropertyPostResponse> propertyPostDetails, SearchPropertyRequest searchPropertyRequest)
        {
            if (propertyPostDetails == null) 
            {
                return null;
            }
            List<PropertyPostResponse> propertyPostResponseList = propertyPostDetails.ToList();
            if (!propertyPostResponseList.Any())
            {
                return propertyPostResponseList;
            }
            foreach(var SortingService in this.criteriaSortingServices)
            {
                await SortingService.Sort(propertyPostResponseList, searchPropertyRequest).ConfigureAwait(false);
            }
            return propertyPostResponseList;
        }
    }
}
