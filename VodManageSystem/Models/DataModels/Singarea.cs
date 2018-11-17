using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VodManageSystem.Models.DataModels
{
    public partial class Singarea
    {
        // removed on 2018-11-15
        public Singarea()
        {
            Singers = new HashSet<Singer>();
        }

        // [Key]
        // [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string AreaNo { get; set; }
        public string AreaNa { get; set; }
        public string AreaEn { get; set; }

        // [InverseProperty(nameof(Singer.Singarea))]
        // public virtual ICollection<Singer> Singers { get; set; }

        // the following is removed on 2018-11-15
        public ICollection<Singer> Singers { get; set; }
    }
}
