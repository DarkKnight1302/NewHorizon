using NewHorizon.Models.ColleagueCastleModels;
using NewHorizon.Services.ColleagueCastleServices.Interfaces;

namespace NewHorizon.Services.ColleagueCastleServices
{
    public class PropertyMatchingService : IPropertyMatchingService
    {
        private readonly IEnumerable<ICriteriaMatching> CriteriaMatchingServices;

        public PropertyMatchingService(IServiceProvider serviceProvider)
        {
            this.CriteriaMatchingServices = serviceProvider.GetServices<ICriteriaMatching>();
        }
        public async Task<bool> IsMatch(PropertyPostDetails propertyPostDetails, SearchPropertyRequest searchPropertyRequest)
        {
            foreach (var CriteriaMatch in this.CriteriaMatchingServices)
            {
                bool isMatch = await CriteriaMatch.IsMatch(propertyPostDetails, searchPropertyRequest).ConfigureAwait(false);
                if (!isMatch)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
