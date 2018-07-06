using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MarielAPI.Models.DTO.Account
{
    public class AccountBaseDTO
    {
        [JsonProperty(Order = 2)]
        public string phoneNumber { get; set; }

        [JsonProperty(Order = 3)]
        public string email { get; set; }
    }
}