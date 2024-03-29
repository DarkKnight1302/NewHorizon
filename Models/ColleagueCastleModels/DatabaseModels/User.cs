﻿using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;

namespace NewHorizon.Models.ColleagueCastleModels.DatabaseModels
{
    public class User
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "uid")]
        public string UserName { get; set; }

        public bool IsAdmin { get; set; }

        public string Salt { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string HashedPassword { get; set; }

        public string Company { get; set; }

        public int ExperienceInYears { get; set; }

        public string CorporateEmailHash { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
