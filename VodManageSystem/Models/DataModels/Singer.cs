using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VodManageSystem.Models.DataModels
{
    public partial class Singer
    {
        public Singer()
        {
            SongSinger1s = new HashSet<Song>();
            SongSinger2s = new HashSet<Song>();
        }

        // [Key]
        // [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string SingNo { get; set; }
        public string SingNa { get; set; }
        public uint NumFw { get; set; }
        public string NumPw { get; set; }
        public string Sex { get; set; }
        public string Chor { get; set; }
        public string Hot { get; set; }
        public int AreaId { get; set; }
        public string PicFile { get; set; }

        // [ForeignKey(nameof(AreaId))]
        // public virtual Singarea Singarea { get; set; }
        public Singarea Singarea { get; set; }

        // [InverseProperty(nameof(Song.Singer1))]
        // public virtual ICollection<Song> SongSinger1s { get; set; }
        public ICollection<Song> SongSinger1s { get; set; }

        // [InverseProperty(nameof(Song.Singer2))]
        // public virtual ICollection<Song> SongSinger2s { get; set; }
        public ICollection<Song> SongSinger2s { get; set; }
    }
}
