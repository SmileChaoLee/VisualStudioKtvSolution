using System;
namespace VodManageSystem.Models
{
    /// <summary>
    /// Song state of requests.
    /// keeps some values for different requests in same HTTP session
    /// </summary>
    public class SongStateOfRequest
    {
        public int CurrentPageNo { get; set; }
        public int OrgId { get; set; }
        public string OrgSongNo { get; set; }
        public string OrderBy { get; set; }
        public string QueryCondition { get; set; }
        public int FirstSongId { get; set; }
        public DateTime StartTime { get; set; }

        public SongStateOfRequest()
        {
            CurrentPageNo = 1;
            OrgId = 0;  // there is no Id = 0 in Song table
            OrgSongNo = "";
            OrderBy = "SongNo";
            QueryCondition = "";
            FirstSongId = 0;    // record the Id of first song in this page
            StartTime = DateTime.Now;
        }
    }
}
