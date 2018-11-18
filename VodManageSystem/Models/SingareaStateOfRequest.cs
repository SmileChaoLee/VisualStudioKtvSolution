﻿using System;
namespace VodManageSystem.Models
{
    public class SingareaStateOfRequest
    {
        public int CurrentPageNo { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public int OrgId { get; set; }
        public string OrgAreaNo { get; set; }
        public string OrderBy { get; set; }
        public string QueryCondition { get; set; }
        public int FirstId { get; set; }
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:VodManageSystem.Models.SingareaStateOfRequest"/> class.
        /// </summary>
        public SingareaStateOfRequest()
        {
            CurrentPageNo = 1;
            PageSize = 15;  // default value for View
            TotalRecords = 0;
            TotalPages = 0;
            OrgId = 0;  // there is no Id = 0 in Singarea table
            OrgAreaNo = "";
            OrderBy = "AreaNo";
            QueryCondition = "";
            FirstId = 0;    // record the Id of first Singarea in this page
            StartTime = DateTime.Now;
        }
    }
}