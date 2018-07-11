using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MarielAPI.Models.DTO.LoanRequest
{
    public class LoanRequestViewDTO: LoanRequestBaseDTO
    {
        [JsonProperty(Order = 1)]
        public int id { get; set; }
    }
}