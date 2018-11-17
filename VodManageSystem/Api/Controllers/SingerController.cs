using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using VodManageSystem.Models;
using VodManageSystem.Models.Dao;
using VodManageSystem.Models.DataModels;
using VodManageSystem.Utilities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VodManageSystem.Api.Controllers
{
    [Route("api/[controller]")]
    public class SingerController : Controller
    {
        private readonly KtvSystemDBContext _context;
        private readonly SingerManager _singerManager;
        private readonly SingareaManager _singareaManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:VodManageSystem.Controllers.SingerController"/> class.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="singerManager">Singer manager.</param>
        /// <param name="singareaManager">Singer Area manager.</param>
        public SingerController(KtvSystemDBContext context, SingerManager singerManager, SingareaManager singareaManager)
        {
            _context = context;
            _singerManager = singerManager;
            _singareaManager = singareaManager;
        }

        // GET: api/values
        [HttpGet]
        public async Task<string> Get()
        {
            Console.WriteLine("Get all singers.");

            // It is too many so only the first 50 records
            SingerStateOfRequest singerState = new SingerStateOfRequest();
            singerState.PageSize = 50;
            singerState.CurrentPageNo = 1;
            singerState.OrgSingNo = "SingNo";   // order by singer's No
            List<Singer> singers = await _singerManager.GetOnePageOfSingers(singerState);
            foreach(var singer in singers){
                // set the properties that are not needed to null
                singer.Singarea = null;
                singer.SongSinger1s = null;
                singer.SongSinger2s = null;
            }

            // convert singers object to JSON string (serializing)
            string singersString = JsonUtil.SetJsonStringFromObject(singers);

            JObject jObject = new JObject();
            jObject.Add("PageNo", singerState.CurrentPageNo);
            jObject.Add("PageSize", singerState.PageSize);
            JArray jArray = JArray.Parse(singersString);
            jObject.Add("Singers", jArray);

            return jObject.ToString();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<Singer> Get(int id)
        {
            // get one singer
            Console.WriteLine("[HttpGet(\"{id}\")]");
            return await _singerManager.FindOneSingerById(id);
        }

        // GET api/values/10/1
        [HttpGet("{pageSize}/{pageNo}/{orderBy}")]
        public async Task<string> Get(int pageSize, int pageNo, string orderBy) {
            Console.WriteLine("HttpGet[\"{ pageSize}/{ pageNo}/{orderBy}\")]");

            // orderBy is either "SingNo" or "SingNa"
            string orderByParam;
            if (string.IsNullOrEmpty(orderBy))
            {
                orderByParam = "SingNo"; // default value is "SingNo"
            }
            else
            {
                string orderByTemp = orderBy.ToUpper();
                if (orderByTemp == "SINGNO")
                {
                    orderByParam = "SingNo";
                }
                else if (orderByTemp == "SINGNA")
                {
                    orderByParam = "SingNa";
                }
                else
                {
                    orderByParam = "SingNo";
                }
            }

            SingerStateOfRequest singerState = new SingerStateOfRequest();
            singerState.PageSize = pageSize;
            singerState.CurrentPageNo = pageNo;
            singerState.OrderBy = orderByParam;
            List<Singer> singers = await _singerManager.GetOnePageOfSingers(singerState);
            foreach (var singer in singers)
            {
                // set the properties that are not needed to null
                singer.Singarea = null;
                singer.SongSinger1s = null;
                singer.SongSinger2s = null;
            }

            // convert singers object to JSON string (serializing)
            string singersString = JsonUtil.SetJsonStringFromObject(singers);

            JObject jObject = new JObject();
            jObject.Add("PageNo", singerState.CurrentPageNo);
            jObject.Add("PageSize", singerState.PageSize);
            JArray jArray = JArray.Parse(singersString);
            jObject.Add("Singers", jArray);

            return jObject.ToString();
        }

        // GET api/values/5/"1"/10/1
        [HttpGet("{areaId}/{sex}/{pageSize}/{pageNo}/{orderBy}")]
        public async Task<string> Get(int areaId, String sex, int pageSize, int pageNo, string orderBy)
        {
            Console.WriteLine("HttpGet[\"{areaId}/{sex}/{ pageSize}/{ pageNo}/{orderBy}\")]");
            Console.WriteLine("areaId = {0}, sex = {1}, pageSize = {2}, pageNo = {3}, orderBy = {4}", areaId, sex, pageSize, pageNo, orderBy);

            // orderBy is either "SingNo" or "SingNa"
            string orderByParam;
            if (string.IsNullOrEmpty(orderBy))
            {
                orderByParam = "SingNo"; // default value is "SingNo"
            }
            else
            {
                string orderByTemp = orderBy.ToUpper();
                if (orderByTemp == "SINGNO")
                {
                    orderByParam = "SingNo";
                }
                else if (orderByTemp == "SINGNA")
                {
                    orderByParam = "SingNa";
                }
                else
                {
                    orderByParam = "SingNo";
                }
            }

            SingerStateOfRequest singerState = new SingerStateOfRequest();
            singerState.PageSize = pageSize;
            singerState.CurrentPageNo = pageNo;
            singerState.OrderBy = orderByParam;
            List<Singer> singers = await _singerManager.GetOnePageOfSingersByAreaSex(singerState, areaId, sex);
            foreach (var singer in singers)
            {
                // set the properties that are not needed to null
                singer.Singarea = null;
                singer.SongSinger1s = null;
                singer.SongSinger2s = null;
            }

            // convert singers object to JSON string (serializing)
            string singersString = JsonUtil.SetJsonStringFromObject(singers);

            JObject jObject = new JObject();
            jObject.Add("PageNo",singerState.CurrentPageNo);
            jObject.Add("PageSize", singerState.PageSize);
            JArray jArray = JArray.Parse(singersString);
            jObject.Add("Singers", jArray);

            return jObject.ToString();
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
