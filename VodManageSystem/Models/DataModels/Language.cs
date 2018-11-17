using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VodManageSystem.Models.DataModels
{
    public partial class Language
    {
        public Language()
        {
            Songs = new HashSet<Song>();
        }
        // [Key]
        // [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string LangNo { get; set; }
        public string LangNa { get; set; }
        public string LangEn { get; set; }

        // [InverseProperty(nameof(Song.Language))]
        // public virtual ICollection<Song> Songs { get; set; }
        public ICollection<Song> Songs { get; set; }
    }
}
