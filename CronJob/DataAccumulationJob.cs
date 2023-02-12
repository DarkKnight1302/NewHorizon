using NewHorizon.Controllers;
using NewHorizon.Helpers;
using NewHorizon.Models;
using NewHorizon.Services.Interfaces;
using NewHorizon.Utils;
using System.Timers;

namespace NewHorizon.CronJob
{
    public class DataAccumulationJob : IDataAccumulationJob
    {
        private ILogger<TrafficDurationController> logger;
        private IRouteRegistrationService routeRegistrationService;
        private ITrafficDataService trafficDataService;
        private const int intervalInMins = 15;
        private const double IntervalInMilliseconds = intervalInMins * 60 * 1000;

        public DataAccumulationJob(ILogger<TrafficDurationController> logger, IRouteRegistrationService routeRegistrationService, ITrafficDataService trafficDataService)
        {
            this.logger = logger;
            this.routeRegistrationService = routeRegistrationService;
            this.trafficDataService = trafficDataService;
        }

        public async void Init()
        {
            System.Timers.Timer t = new System.Timers.Timer();
            t.AutoReset = true;
            int currentMinuteOffset = DateTimeOffset.UtcNow.Minute % intervalInMins;
            if (currentMinuteOffset == 0)
            {
                t.Interval = IntervalInMilliseconds;
                t.Enabled = true;
            }
            else
            {
                int delayTime = intervalInMins - currentMinuteOffset;
                await Task.Delay(delayTime * 60 * 1000);
                t.Interval = IntervalInMilliseconds;
                t.Enabled = true;
            }
            t.Elapsed += StartRouteDataProcessing;
        }

        private async void StartRouteDataProcessing(object? sender, ElapsedEventArgs e)
        {
            this.logger.LogInformation($"Cron running at {DateTimeOffset.UtcNow.ToIndiaTime()}");
            DateTimeOffset currentTime = DateTimeOffset.UtcNow.ToIndiaTime();
            TimeOfDay timeOfDay = new TimeOfDay(currentTime.Hour, currentTime.Minute);
            IReadOnlyCollection<RegisteredRoutes> routes = await this.routeRegistrationService.GetAllRoutesForDateTimeAsync(currentTime).ConfigureAwait(false);

            if (routes == null)
            {
                return;
            }

            foreach (RegisteredRoutes route in routes)
            {
                int trafficTime = this.trafficDataService.FetchAndStoreRouteTimeInMinutes(route.fromPlaceId, route.toPlaceId, (int)currentTime.DayOfWeek, timeOfDay);
                this.logger.LogInformation($"Traffic time duration in mins {trafficTime}");
            }
        }
    }
}
