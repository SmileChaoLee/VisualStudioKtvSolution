using System;
namespace VodManageSystem.Models.DataModels
{
    public partial class Singarea
    {
        /// <summary>
        /// Copies from another song.
        /// </summary>
        /// <param name="singarea">Language.</param>
        public void CopyFrom(Singarea singarea)
        {
            Id = singarea.Id;
            AreaNo = singarea.AreaNo;
            AreaNa = singarea.AreaNa;
            AreaEn = singarea.AreaEn;
        }
    }
}
