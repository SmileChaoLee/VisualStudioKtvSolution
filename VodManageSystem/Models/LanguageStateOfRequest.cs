using System;
namespace VodManageSystem.Models
{
    /// <summary>
    /// Language state of requests.
    /// keeps some values for different requests in same HTTP session
    /// </summary>
    public class LanguageStateOfRequest
    {
        public int CurrentPageNo { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public int OrgId { get; set; }
        public string OrgLangNo { get; set; }
        public string OrderBy { get; set; }
        public string QueryCondition { get; set; }
        public int FirstId { get; set; }
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:VodManageSystem.Models.LanguageStateOfRequest"/> class.
        /// </summary>
        public LanguageStateOfRequest()
        {
            CurrentPageNo = 1;
            PageSize = 15;  // default value for View
            TotalRecords = 0;
            TotalPages = 0;
            OrgId = 0;  // there is no Id = 0 in Language table
            OrgLangNo = "";
            OrderBy = "LangNo";
            QueryCondition = "";
            FirstId = 0;    // record the Id of first Language in this page
            StartTime = DateTime.Now;
        }
    }
}
