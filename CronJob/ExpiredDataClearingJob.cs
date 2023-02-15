using NewHorizon.Services.ColleagueCastleServices.Interfaces;
using NewHorizon.Utils;

namespace NewHorizon.CronJob
{
    public class ExpiredDataClearingJob : IExpiredDataClearingJob
    {
        private const double IntervalInMilliseconds = 60 * 60 * 1000;
        private const int beginHour = 3;
        private const int endHour = 5;
        private readonly IEnumerable<IClearExpiredData> clearDataServices;

        public ExpiredDataClearingJob(IServiceProvider serviceProvider)
        {
            this.clearDataServices = serviceProvider.GetServices<IClearExpiredData>();
        }

        public void Init()
        {
            System.Timers.Timer t = new System.Timers.Timer();
            t.AutoReset = true;
            t.Interval = IntervalInMilliseconds;
            t.Enabled= true;
            t.Elapsed += PeriodicCleaner;
        }

        private async void PeriodicCleaner(object? sender, System.Timers.ElapsedEventArgs e)
        {
            DateTimeOffset currentTime = DateTimeOffset.UtcNow.ToIndiaTime();
            if (currentTime.Hour >= beginHour && currentTime.Hour <= endHour)
            {
                foreach(var clearingService in clearDataServices)
                {
                    await clearingService.ClearData().ConfigureAwait(false);
                }
            }
        }
    }
}
