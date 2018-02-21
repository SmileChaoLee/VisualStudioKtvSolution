using System;
using Xunit;

using VodManageSystem;
using VodManageSystem.Models;
using VodManageSystem.Utilities;
using VodManageSystem.ExtensionMethods;
using VodManageSystem.Models.DataModels;
using VodManageSystem.Models.Dao;

namespace VoManageTest
{
    [Collection("ProjectCollection")]
    public class UnitTestForVodManageSystem
    {
        //private readonly KtvSystemDBContext _context;
        //private readonly SongManager _songManager;
        public UnitTestForVodManageSystem()
        {
        }

        [Fact]
        public void Test1_JsongUtil()
        {
            string jsonString = "{\"CurrentPageNo\":1,\"OrgId\":0,\"OrgSongNo\":\"\",\"OrderBy\":\"SongNo\",\"QueryCondition\":\"\"}";
            SongStateOfRequest songState = new SongStateOfRequest();
            string result = JsonUtil.SetJsonStringFromObject(songState);
            Assert.False(!result.Equals(jsonString), jsonString + " is not a Json string from SongStateOfRequest object");
        }
        [Fact]
        public void Test2_JsongUtil()
        {
            string jsonString = "{\"CurrentPageNo\":1,\"OrgId\":0,\"OrgSongNo\":\"\",\"OrderBy\":\"\",\"QueryCondition\":\"\"}";;
            SongStateOfRequest songState = new SongStateOfRequest();
            string result = JsonUtil.SetJsonStringFromObject(songState);
            Assert.False(!result.Equals(jsonString), jsonString + " is not a Json string from SongStateOfRequest object");
        }
    }
}
