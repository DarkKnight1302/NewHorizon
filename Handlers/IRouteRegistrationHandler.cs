namespace NewHorizon.Handlers
{
    public interface IRouteRegistrationHandler
    {
        Task RegisterRoute(string fromPlaceId, string toPlaceId, DateTimeOffset startTimeUtc, DateTimeOffset endTimeUtc);
    }
}
