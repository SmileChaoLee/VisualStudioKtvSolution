using System;
namespace VodManageSystem.Models
{
    public class PlayerscoreStateOfRequest
    {
        public int CurrentPageNo { get; set; }
        public int OrgId { get; set; }
        public string OrgPlayerName { get; set; }
        public string OrderBy { get; set; }
        public string QueryCondition { get; set; }
        public int FirstId { get; set; }
        public DateTime StartTime { get; set; }

        public PlayerscoreStateOfRequest()
        {
            CurrentPageNo = 1;
            OrgId = 0;  // there is no Id = 0 in Singer table
            OrgPlayerName = "";
            OrderBy = "PlayerName";
            QueryCondition = "";
            FirstId = 0;    // record the Id of first Singer in this page
            StartTime = DateTime.Now;   
        }
    }
}
