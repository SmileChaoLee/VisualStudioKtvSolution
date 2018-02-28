using System;
namespace VodManageSystem.Models.DataModels
{
    /// <summary>
    /// Language extension.
    /// </summary>
    public partial class Language
    {
        /// <summary>
        /// Copies from another song.
        /// </summary>
        /// <param name="language">Language.</param>
        public void CopyFromAnotherLanguage(Language language)
        {
            Id = language.Id;
            LangNo = language.LangNo;
            LangNa = language.LangNa;
            LangEn = language.LangEn;
        }
    }
}
