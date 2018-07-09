using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MarielAPI.Models.DTO.Account
{
    public class AccountBaseDTO
    {
        [JsonProperty(Order = 2)]
        [Required]
        [MaxLength(100)]
        public string phoneNumber { get; set; }

        [JsonProperty(Order = 3)]
        [MaxLength(100)]
        public string email { get; set; }
    }
}