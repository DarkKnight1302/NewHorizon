﻿using NewHorizon.Models.ColleagueCastleModels;
using NewHorizon.Models.ColleagueCastleModels.DatabaseModels;

namespace NewHorizon.Services.ColleagueCastleServices.Interfaces
{
    public interface ISearchPropertyService
    {
        public Task<(IEnumerable<PropertyPostResponse>?, string error)> GetMatchingPropertyListAsync(SearchPropertyRequest searchPropertyRequest);
    }
}
