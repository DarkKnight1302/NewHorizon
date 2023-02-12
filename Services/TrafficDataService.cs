using NewHorizon.Helpers;
using NewHorizon.Models;
using NewHorizon.Services.Interfaces;
using NewHorizon.Utils;
using Newtonsoft.Json;
using System.Net.Http;

namespace NewHorizon.Services
{
    public class TrafficDataService : ITrafficDataService
    {
        private string ApiKey;
        private HttpClient httpClient;
        private const int MaxRequestCountPerDay = 500;
        private readonly ITrafficDataStorageService trafficDataStorageService;
        private RequestThresholdPerDay requestThresholdPerDay;

        public TrafficDataService(ITrafficDataStorageService trafficDataStorageService, ISecretService secretService) 
        {
            httpClient = new HttpClient();
            this.trafficDataStorageService= trafficDataStorageService;
            this.requestThresholdPerDay = new RequestThresholdPerDay(MaxRequestCountPerDay);
            this.ApiKey = secretService.GetSecretValue("GOOGLE_MATRIX_APL_KEY");
        }
        public int FetchAndStoreRouteTimeInMinutes(string fromPlaceId, string toPlaceId, int dayOfWeek, TimeOfDay timeOfDay)
        {
            if (!this.requestThresholdPerDay.AllowRequest())
            {
                return -1;
            }
            HttpResponseMessage response = httpClient.GetAsync($"https://maps.googleapis.com/maps/api/distancematrix/json?destinations=place_id:{toPlaceId}&origins=place_id:{fromPlaceId}&key={ApiKey}&departure_time=now").Result;
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                DistanceApiResponse apiResponse = JsonConvert.DeserializeObject<DistanceApiResponse>(result);
                int trafficTime = apiResponse?.rows[0]?.elements[0]?.duration_in_traffic.value ?? -1;
                trafficDataStorageService.StoreRouteData(fromPlaceId, toPlaceId, dayOfWeek, timeOfDay, trafficTime/60);
                return (int)(trafficTime/60);
            }
            return -1;
        }
    }
}
