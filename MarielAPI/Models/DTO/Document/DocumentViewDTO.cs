using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MarielAPI.Models.DTO.Document
{
    public class DocumentViewDTO
    {
        public int id { get; set; }
        public string name { get; set; }
        public string path { get; set; }
    }
}