using Microsoft.Azure.Cosmos;
using NewHorizon.Helpers;
using NewHorizon.Models;
using NewHorizon.Services.Interfaces;
using System;
using System.Collections.Generic;

namespace NewHorizon.Services
{
    public class TrafficDataStorageService : ITrafficDataStorageService
    {
        private readonly Container container;
        private readonly ILogger<TrafficDataStorageService> logger;
        private const int maxDataSize = 96;

        public TrafficDataStorageService(ILogger<TrafficDataStorageService> logger, ICosmosDbService cosmosDbService)
        {
            this.container = cosmosDbService.GetContainer("RouteTrafficData", "RouteTime");
            this.logger = logger;
        }

        public async Task<Dictionary<string, int>> GetMedianRouteTimeAsync(string fromPlaceId, string toPlaceId, int dayofWeek)
        {
            string identifier = fromPlaceId + "_" + toPlaceId + "_" + dayofWeek;
            RouteDbData routeDbData;
            try
            {
                ItemResponse<RouteDbData> responseItem = await this.container.ReadItemAsync<RouteDbData>(identifier, new PartitionKey(identifier)).ConfigureAwait(false);
                routeDbData = responseItem.Resource;
            }
            catch (CosmosException)
            {
                return null;
            }
            Dictionary<string, int> result = new Dictionary<string, int>();
            foreach(KeyValuePair<string, List<int>> kv in routeDbData.routeTimeData) 
            {
                result.TryAdd(kv.Key, this.FindMedian(kv.Value));
            }
            return result;
        }

        public async void StoreRouteData(string fromPlaceId, string toPlaceId, int dayOfWeek, TimeOfDay timeOfDay, int timeInMins)
        {
            string identifier = fromPlaceId+ "_" + toPlaceId+ "_" + dayOfWeek;
            RouteDbData routeDbData;
            try
            {
                ItemResponse<RouteDbData> responseItem = await this.container.ReadItemAsync<RouteDbData>(identifier, new PartitionKey(identifier));
                routeDbData = responseItem.Resource;
            } catch(CosmosException)
            {
                routeDbData = null;
                // element not found. Do nothing.
            }
            if (routeDbData == null)
            {
                routeDbData = new RouteDbData
                {
                    Id = identifier,
                    Uid = identifier,
                };
            }
            if (routeDbData.routeTimeData.Count > maxDataSize)
            {
                this.logger.LogError("Size limit exceeded for dictionary");
                return;
            }
            List<int> durationList = null;
            if (routeDbData.routeTimeData.TryGetValue(timeOfDay.ToString(), out List<int> output))
            {
                durationList = output;
            }
            if (durationList == null)
            {
                durationList = new List<int>();
            }
            durationList.Add(timeInMins);
            routeDbData.routeTimeData[timeOfDay.ToString()] = durationList;
            ItemResponse<RouteDbData> response = await this.container.UpsertItemAsync<RouteDbData>(routeDbData).ConfigureAwait(false);
            if (response?.StatusCode == System.Net.HttpStatusCode.Created || (response?.StatusCode == System.Net.HttpStatusCode.OK))
            {
                this.logger.LogInformation("Saved successfully");
            } else
            {
                this.logger.LogInformation("Failed to save");
            }
        }

        private int FindMedian(List<int> listOfMins)
        {
            int median;
            listOfMins.Sort();
            if (listOfMins.Count % 2 == 1)
            {
                median = listOfMins[listOfMins.Count / 2];
            } else
            {
                int middle1 = listOfMins[listOfMins.Count / 2 - 1];
                int middle2 = listOfMins[listOfMins.Count / 2];
                median = (middle1 + middle2) / 2;
            }
            return median;
        }
    }
}
