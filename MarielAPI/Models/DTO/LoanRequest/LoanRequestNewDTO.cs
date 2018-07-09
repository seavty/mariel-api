using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MarielAPI.Models.DTO.LoanRequest
{
    public class LoanRequestNewDTO : LoanRequestBaseDTO
    {
        [Required]
        public int  accountID { get; set; }

    }
}