using System;
namespace VodManageSystem.Models
{
    public class SingerStateOfRequest
    {
        public int CurrentPageNo { get; set; }
        public int OrgId { get; set; }
        public string OrgSingNo { get; set; }
        public string OrderBy { get; set; }
        public string QueryCondition { get; set; }
        public int FirstSingId { get; set; }
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:VodManageSystem.Models.SingerStateOfRequest"/> class.
        /// </summary>
        public SingerStateOfRequest()
        {
            CurrentPageNo = 1;
            OrgId = 0;  // there is no Id = 0 in Language table
            OrgSingNo = "";
            OrderBy = "SingNo";
            QueryCondition = "";
            FirstSingId = 0;    // record the Id of first Language in this page
            StartTime = DateTime.Now;
        }
    }
}
