﻿using GoogleApi.Entities.Common.Enums;
using GoogleApi.Entities.Places.Details.Response;

namespace NewHorizon.Utils
{
    public static class CityNameUtil
    {
        public static string GetCityNameFromPlaceDetails(DetailsResult placeDetails)
        {
            string cityName = null;

            foreach (var addressComponent in placeDetails.AddressComponents)
            {
                foreach (var componentType in addressComponent.Types)
                {
                    if (AddressComponentType.Locality == componentType)
                    {
                        cityName = addressComponent.LongName;
                        break;
                    }
                }
            }
            return cityName;
        }
    }
}
