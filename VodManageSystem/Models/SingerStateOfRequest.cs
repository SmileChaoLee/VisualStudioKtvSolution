using System;
namespace VodManageSystem.Models
{
    public class SingerStateOfRequest
    {
        public int CurrentPageNo { get; set; }
        public int PageSize { get; set; }
        public int OrgId { get; set; }
        public string OrgSingNo { get; set; }
        public string OrderBy { get; set; }
        public string QueryCondition { get; set; }
        public int FirstId { get; set; }
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:VodManageSystem.Models.SingerStateOfRequest"/> class.
        /// </summary>
        public SingerStateOfRequest()
        {
            CurrentPageNo = 1;
            PageSize = 15;  // default value for View
            OrgId = 0;  // there is no Id = 0 in Singer table
            OrgSingNo = "";
            OrderBy = "SingNo";
            QueryCondition = "";
            FirstId = 0;    // record the Id of first Singer in this page
            StartTime = DateTime.Now;
        }
    }
}
