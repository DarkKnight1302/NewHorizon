using Microsoft.AspNetCore.DataProtection;
using Microsoft.Azure.Cosmos;
using NewHorizon.Helpers;
using NewHorizon.Models;
using NewHorizon.Services.Interfaces;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;

namespace NewHorizon.Services
{
    public class RouteRegistrationService : IRouteRegistrationService
    {
        private readonly Container container;
        private readonly ILogger<TrafficDataStorageService> logger;

        public RouteRegistrationService(ILogger<TrafficDataStorageService> logger, ICosmosDbService cosmosDbService)
        {
            this.container = cosmosDbService.GetContainer("RouteTrafficData", "RegisteredRoutes");
            this.logger = logger;
        }

        public async Task<IReadOnlyCollection<RegisteredRoutes>> GetAllRoutesForDateTimeAsync(DateTimeOffset dateTime)
        {
            List<RegisteredRoutes> allRoutes = await this.GetAllRoutes().ConfigureAwait(false);
            ConcurrentDictionary<string, RegisteredRoutes> inRangeRoutes = new ConcurrentDictionary<string, RegisteredRoutes>();
            List<Task> tasks = new List<Task>();
            foreach (RegisteredRoutes route in allRoutes)
            {
                tasks.Add(Task.Run(() =>
                {
                    List<Tuple<DateTimeOffset, DateTimeOffset>> registeredTimeSlots = route.registeredTimeSlots;
                    foreach (Tuple<DateTimeOffset, DateTimeOffset> timeSlot in registeredTimeSlots)
                    {
                        var startTime = timeSlot.Item1;
                        var endTime = timeSlot.Item2;
                        if (this.IfTimeLiesInRange(startTime, endTime, dateTime))
                        {
                            inRangeRoutes.TryAdd(route.Id, route);
                        }
                    }
                }));
            }
            await Task.WhenAll(tasks).ConfigureAwait(false);
            return new ReadOnlyCollection<RegisteredRoutes>(inRangeRoutes.Values.ToList());
        }

        public async Task<List<RegisteredRoutes>> GetAllRoutes()
        {
            List<RegisteredRoutes> registeredRoutes = new List<RegisteredRoutes>();
            var iterator = this.container.GetItemQueryIterator<RegisteredRoutes>();
            while (iterator.HasMoreResults)
            {
                FeedResponse<RegisteredRoutes> response = await iterator.ReadNextAsync().ConfigureAwait(false);
                var enumerator = response.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    registeredRoutes.Add(enumerator.Current);
                }
            }
            return registeredRoutes;
        }

        public async Task RegisterRoute(string fromPlaceId, string toPlaceID, DateTimeOffset startTime, DateTimeOffset endTime)
        {
            string identifier = fromPlaceId + "_" + toPlaceID;
            RegisteredRoutes registeredRoutes = null;
            try
            {
                ItemResponse<RegisteredRoutes> itemResponse = await this.container.ReadItemAsync<RegisteredRoutes>(identifier, new PartitionKey(identifier)).ConfigureAwait(false);
                registeredRoutes = itemResponse.Resource;
            }
            catch (CosmosException)
            {
                registeredRoutes = null;
            }

            if (registeredRoutes == null)
            {
                registeredRoutes = new RegisteredRoutes
                {
                    Id = identifier,
                    Uid = identifier,
                    fromPlaceId = fromPlaceId,
                    toPlaceId = toPlaceID,
                };
            }

            if (this.IfSameTimeSlotExists(registeredRoutes, new Tuple<DateTimeOffset, DateTimeOffset>(startTime, endTime)))
            {
                return;
            }

            registeredRoutes.registeredTimeSlots.Add(new Tuple<DateTimeOffset, DateTimeOffset>(startTime, endTime));
            ItemResponse<RegisteredRoutes> response = await this.container.UpsertItemAsync<RegisteredRoutes>(registeredRoutes).ConfigureAwait(false);
            if (response?.StatusCode == System.Net.HttpStatusCode.Created || (response?.StatusCode == System.Net.HttpStatusCode.OK))
            {
                this.logger.LogInformation("Saved successfully");
            }
            else
            {
                this.logger.LogInformation("Failed to save");
            }
        }

        private bool IfTimeLiesInRange(DateTimeOffset startTime, DateTimeOffset endTime, DateTimeOffset currentTime)
        {
            TimeOfDay currentTimeOfDay = new TimeOfDay(currentTime.Hour, currentTime.Minute);
            TimeOfDay startTimeOfDay = new TimeOfDay(startTime.Hour, startTime.Minute);
            TimeOfDay endTimeOfDay = new TimeOfDay(endTime.Hour, endTime.Minute);
            return currentTimeOfDay <= endTimeOfDay && currentTimeOfDay >= startTimeOfDay;
        }

        private bool IfSameTimeSlotExists(RegisteredRoutes registeredRoutes, Tuple<DateTimeOffset, DateTimeOffset> newSlot)
        {
            return registeredRoutes.registeredTimeSlots.Where(t => (t.Item1.ToString("HH:mm") == newSlot.Item1.ToString("HH:mm") && t.Item2.ToString("HH:mm") == newSlot.Item2.ToString("HH:mm"))).Count() > 0;
        }
    }
}
