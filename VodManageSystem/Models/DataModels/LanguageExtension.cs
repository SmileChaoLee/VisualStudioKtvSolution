using System;
namespace VodManageSystem.Models.DataModels
{
    /// <summary>
    /// Language extension.
    /// </summary>
    public partial class Language
    {
        /// <summary>
        /// Copies from another language.
        /// </summary>
        /// <param name="language">Language.</param>
        public void CopyColumnsFrom(Language language)
        {
            Id = language.Id;
            LangNo = language.LangNo;
            LangNa = language.LangNa;
            LangEn = language.LangEn;
        }

        /// <summary>
        /// Copies from.
        /// </summary>
        /// <param name="language">Language.</param>
        public void CopyFrom(Language language)
        {
            CopyColumnsFrom(language);
        }
    }
}
