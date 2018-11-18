﻿using System;
namespace VodManageSystem.Models
{
    /// <summary>
    /// Song state of requests.
    /// keeps some values for different requests in same HTTP session
    /// </summary>
    public class SongStateOfRequest
    {
        public int CurrentPageNo { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public int OrgId { get; set; }
        public string OrgSongNo { get; set; }
        public string OrderBy { get; set; }
        public string QueryCondition { get; set; }
        public int FirstId { get; set; }
        public DateTime StartTime { get; set; }

        public SongStateOfRequest()
        {
            CurrentPageNo = 1;
            PageSize = 15;  // default value for View
            TotalRecords = 0;   // no records
            TotalPages = 0; // total pages = 0
            OrgId = 0;  // there is no Id = 0 in Song table
            OrgSongNo = "";
            OrderBy = "SongNo";
            QueryCondition = "";
            FirstId = 0;    // record the Id of first song in this page
            StartTime = DateTime.Now;
        }
    }
}