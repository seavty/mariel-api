using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MarielAPI.Models.DTO.LoanRequest
{
    public class LoanRequestBaseDTO
    {
        [Required]
        public double amount { get; set; }

        [Required]
        public int payday { get; set; }

        [Required]
        public string purpose { get; set; }
    }
}