using System;
namespace VodManageSystem.Models
{
    public class StateOfRequest
    {
        public int CurrentPageNo { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public int OrgId { get; set; }
        public string OrgNo { get; set; }
        public string OrderBy { get; set; }
        public string QueryCondition { get; set; }
        public int FirstId { get; set; }
        public bool IsFirstAddRecord { get; set; }
        public DateTime StartTime { get; set; }

        public StateOfRequest(string orderBy)
        {
            CurrentPageNo = 1;
            PageSize = 15;  // default value for View
            TotalRecords = 0;
            TotalPages = 0;
            OrgId = 0;  // there is no Id = 0 in Singer table
            OrgNo = "";
            this.OrderBy = orderBy;
            QueryCondition = "";
            FirstId = 0;    // record the Id of first Singer in this page
            IsFirstAddRecord = true;
            StartTime = DateTime.Now;
        }
    }
}
