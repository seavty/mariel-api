using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MarielAPI.Models.DTO.Pin
{
    public class PinViewDTO
    {
        [JsonProperty(Order = 1)]
        [Required]
        public int id { get; set; }

        [JsonProperty(Order = 2)]
        [MaxLength(100)]
        public string pinNumber { get; set; }
    }
}