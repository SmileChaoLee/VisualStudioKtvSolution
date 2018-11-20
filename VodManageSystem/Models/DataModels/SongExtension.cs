using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace VodManageSystem.Models.DataModels
{
    /// <summary>
    /// extension members of Song model
    /// </summary>
    public partial class Song
    {
        /// <summary>
        /// constructor of Song
        /// Initializes a new instance of the <see cref="T:VodManageSystem.Models.DataModels.Song"/> class.
        /// </summary>
        public Song()
        {
            // initialize the properties
            Id = 0; // There will not be 0 for id=0 in Song table 
            SongNo = "";
            SongNa = "";
            SNumWord = 0;   // number of words in song title
            NumFw = 0;      // the behua of the first word of the song
            NumPw = "A";    // the pinyin of the first word of the song 
            LanguageId = 1;
            // LangNo = "01";   // for madarin song
            Singer1Id = 0;
            // Singer1No = "09201"; // singer is unknown. for the first singer
            Singer2Id = 0;
            // Singer2No = "09201"; // singer is unknown. for the second singer
            Chor = "N";     // default
            MMpeg = "12";
            NMpeg = "11";
            VodYn = "Y";    // default is having a vod no.
            VodNo = "";     // default is empty, but if VodYn is "Y", then it must not be empty
            Pathname = "";  // default is empty, but if VodYn is "Y", then it must not be empty
            InDate = DateTime.Now;

            OrdNo = 0;
            OrdOldN = 0;
            OrderNum = 0;
        }

        /// <summary>
        /// Copies the columns from.
        /// </summary>
        /// <param name="song">Song.</param>
        public void CopyColumnsFrom(Song song)
        {
            Id = song.Id;
            SongNo = song.SongNo;
            SongNa = song.SongNa;
            SNumWord = song.SNumWord;
            NumFw = song.NumFw;
            NumPw = song.NumPw;
            SeleTf = song.SeleTf;

            /*
            LanguageId = song.LanguageId;
            if (song.Language != null)
            {
                song.LangNo = song.Language.LangNo;
            }
            LangNo = song.LangNo;

            Singer1Id = song.Singer1Id;
            if (song.Singer1 != null)
            {
                song.Singer1No = song.Singer1.SingNo;
            }
            Singer1No = song.Singer1No;

            Singer2Id = song.Singer2Id;
            if (song.Singer2 != null)
            {
                song.Singer2No = song.Singer2.SingNo;
            }
            Singer2No = song.Singer2No;
            */

            Chor = song.Chor;
            MMpeg = song.MMpeg;
            NMpeg = song.NMpeg;
            VodYn = song.VodYn;
            VodNo = song.VodNo;
            Pathname = song.Pathname;
            InDate = song.InDate;

            OrdNo = song.OrdNo;
            OrdOldN = song.OrdOldN;
            OrderNum = song.OrderNum;

            // the tables that relating to song
            LanguageId = song.LanguageId;
            Singer1Id = song.Singer1Id;
            Singer2Id = song.Singer2Id;
        }

        /// <summary>
        /// Copies from another song.
        /// </summary>
        /// <param name="song">Song.</param>
        public void CopyFrom(Song song)
        {
            CopyColumnsFrom(song);

            Language = song.Language;
            Singer1 = song.Singer1;
            Singer2 = song.Singer2;
        }
        
        // 2017-11-29
        // [NotMapped] has to be here if there is setter
        // ,otherwise it will fail to query

        /// <summary>
        /// Gets lang_no of the language.
        /// </summary>
        /// <value>The value of the lang_no.</value>
        // [NotMapped]
        // public string LangNo { get; set; }

        /// <summary>
        /// Gets lang_na of the language.
        /// </summary>
        /// <value>The value of the lang_na.</value>
        // [NotMapped]
        // public string LangNa { get; set; }

        /// <summary>
        /// Gets the sing_no of the first singer
        /// </summary>
        /// <value>The value of sing_no.</value>
        // [NotMapped]
        // public string Singer1No { get; set; }

        /// <summary>
        /// Gets the sing_na of the first singer
        /// </summary>
        /// <value>The value of sing_na.</value>
        // [NotMapped]
        // public string Singer1Na { get; set; }

        /// <summary>
        /// the name sing_no of the second singer.
        /// </summary>
        /// <value>The value of the sing_no.</value>
        // [NotMapped]
        // public string Singer2No { get; set; }

        /// <summary>
        /// the name sing_na of the second singer.
        /// </summary>
        /// <value>The value of the sing_na.</value>
        // [NotMapped]
        // public string Singer2Na { get; set; }
    }
}
