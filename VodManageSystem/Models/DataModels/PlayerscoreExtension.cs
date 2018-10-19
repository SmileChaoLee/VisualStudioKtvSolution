using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace VodManageSystem.Models.DataModels
{
    public partial class Playerscore
    {
        /// <summary>
        /// Copies from another playerscore
        /// </summary>
        /// <param name="playerscore">.</param>
        public void CopyFrom(Playerscore playerscore)
        {
            PlayerName = playerscore.PlayerName;
            Score = playerscore.Score;
            GameId = playerscore.GameId;
        }
    }
}
