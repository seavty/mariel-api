//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MarielAPI.Models.DB
{
    using System;
    using System.Collections.Generic;
    
    public partial class sm_doc
    {
        public int id { get; set; }
        public string deleted { get; set; }
        public Nullable<int> createdBy { get; set; }
        public Nullable<System.DateTime> createdDate { get; set; }
        public Nullable<int> updatedBy { get; set; }
        public Nullable<System.DateTime> updatedDate { get; set; }
        public Nullable<int> docs_WorkflowID { get; set; }
        public Nullable<int> docs_WorkflowItemID { get; set; }
        public Nullable<int> docs_ZoneID { get; set; }
        public string name { get; set; }
        public string docs_Description { get; set; }
        public Nullable<int> tableID { get; set; }
        public string value { get; set; }
        public string filePath { get; set; }
        public string docs_Default { get; set; }
    }
}
