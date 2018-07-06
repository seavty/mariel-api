using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MarielAPI.Models.DTO.Account
{
    public class AccountCheckInfoDTO
    {
        [Required]
        [MaxLength(100)]
        public string phoneNumber { get; set; }
    }
}