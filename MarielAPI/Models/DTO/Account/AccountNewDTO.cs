using MarielAPI.Models.DTO.LoanRequest;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MarielAPI.Models.DTO.Account
{
    public class AccountNewDTO
    {
        [Required]
        public string idCardBase64 { get; set; }

        [Required]
        public string employmentLetterBase64 { get; set; }

        [Required]
        public string bankAccountBase64 { get; set; }

        [Required]
        public LoanRequestBaseDTO loanRequest { get; set; }
    }
}