using System;
using System.Collections.Generic;

namespace VodManageSystem.Models.DataModels
{
    public partial class User
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserPassword { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserState { get; set; }
    }
}
