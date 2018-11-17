using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VodManageSystem.Models.DataModels
{
    public partial class Song
    {
        public int Id { get; set; }
        public string SongNo { get; set; }
        public string SongNa { get; set; }
        public int LanguageId { get; set; }
        public int? SNumWord { get; set; }
        public int? NumFw { get; set; }
        public string NumPw { get; set; }
        public bool SeleTf { get; set; }
        public string Chor { get; set; }
        public string NMpeg { get; set; }
        public string MMpeg { get; set; }
        public string VodYn { get; set; }
        public string VodNo { get; set; }
        public string Pathname { get; set; }
        public int? OrdNo { get; set; }
        public int? OrderNum { get; set; }
        public int? OrdOldN { get; set; }
        public DateTime? InDate { get; set; }
        public int Singer1Id { get; set; }
        public int Singer2Id { get; set; }

        // [ForeignKey(nameof(LanguageId))]
        // public virtual Language Language { get; set; }
        public Language Language { get; set; }

        // [ForeignKey(nameof(Singer1Id))]
        // public virtual Singer Singer1 { get; set; }
        public Singer Singer1 { get; set; }

        // [ForeignKey(nameof(Singer2Id))]
        // public virtual Singer Singer2 { get; set; }
        public Singer Singer2 { get; set; }
    }
}
