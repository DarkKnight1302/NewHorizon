using NewHorizon.Services.Interfaces;
using NewHorizon.Utils;

namespace NewHorizon.Handlers
{
    public class RouteRegistrationHandler : IRouteRegistrationHandler
    {
        private readonly IRouteRegistrationService routeRegistrationService;

        public RouteRegistrationHandler(IRouteRegistrationService routeRegistrationService) 
        {
            this.routeRegistrationService = routeRegistrationService;
        }

        public async Task RegisterRoute(string fromPlaceId, string toPlaceId, DateTimeOffset startTimeUtc, DateTimeOffset endTimeUtc)
        {
            await this.routeRegistrationService.RegisterRoute(fromPlaceId, toPlaceId, startTimeUtc.ToIndiaTime(), endTimeUtc.ToIndiaTime()).ConfigureAwait(false);
        }
    }
}
