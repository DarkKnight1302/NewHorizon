using NewHorizon.Models.ColleagueCastleModels;
using NewHorizon.Models.ColleagueCastleModels.DatabaseModels;
using NewHorizon.Services.ColleagueCastleServices.Interfaces;
using NewHorizon.Services.Interfaces;

namespace NewHorizon.Services.ColleagueCastleServices
{
    public class PropertySortingService : IPropertySortingService
    {
        private readonly IEnumerable<ICriteriaSorting> criteriaSortingServices;

        public PropertySortingService(IServiceProvider serviceProvider)
        {
            this.criteriaSortingServices = serviceProvider.GetServices<ICriteriaSorting>();
        }

        public async Task SortProperties(IEnumerable<PropertyPostResponse> propertyPostDetails, SearchPropertyRequest searchPropertyRequest)
        {
            if (propertyPostDetails == null || !propertyPostDetails.Any()) 
            {
                return;
            }
            List<PropertyPostResponse> propertyPostResponseList = propertyPostDetails.ToList();
            foreach(var SortingService in this.criteriaSortingServices)
            {
                await SortingService.Sort(propertyPostResponseList, searchPropertyRequest).ConfigureAwait(false);
            }
        }
    }
}
