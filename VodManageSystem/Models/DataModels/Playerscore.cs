using System;
namespace VodManageSystem.Models.DataModels
{
    public partial class Playerscore
    {
        public int Id { get; set; }
        public string PlayerName { get; set; }
        public int Score { get; set; }
        public int? GameId { get; set; }
    }
}
