using System;
namespace VodManageSystem.Models
{
    public class SingareaStateOfRequest
    {
        public int CurrentPageNo { get; set; }
        public int OrgId { get; set; }
        public string OrgAreaNo { get; set; }
        public string OrderBy { get; set; }
        public string QueryCondition { get; set; }
        public int FirstAreaId { get; set; }
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:VodManageSystem.Models.SingareaStateOfRequest"/> class.
        /// </summary>
        public SingareaStateOfRequest()
        {
            CurrentPageNo = 1;
            OrgId = 0;  // there is no Id = 0 in Language table
            OrgAreaNo = "";
            OrderBy = "AreaNo";
            QueryCondition = "";
            FirstAreaId = 0;    // record the Id of first Language in this page
            StartTime = DateTime.Now;
        }
    }
}
