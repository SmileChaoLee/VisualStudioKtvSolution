using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace VodManageSystem.Models.DataModels
{
    public partial class Singer
    {
        /// <summary>
        /// Copies the columns from.
        /// </summary>
        /// <param name="singer">Singer.</param>
        public void CopyColumnsFrom(Singer singer)
        {
            Id = singer.Id;
            SingNo = singer.SingNo;
            SingNa = singer.SingNa;
            NumFw = singer.NumFw;
            NumPw = singer.NumPw;
            Sex = singer.Sex;
            Chor = singer.Chor;
            Hot = singer.Hot;
            AreaId = singer.AreaId;
            PicFile = singer.PicFile;
        }

        /// <summary>
        /// Copies from another singer
        /// </summary>
        /// <param name="singer">Singer.</param>
        public void CopyFrom(Singer singer)
        {
            CopyColumnsFrom(singer);
            Singarea = singer.Singarea;
        }
    }
}
