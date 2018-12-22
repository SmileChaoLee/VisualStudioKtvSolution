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
    public class SingersController : Controller
    {
        private readonly KtvSystemDBContext _context;
        private readonly SingersManager _singersManager;
        private readonly SingareasManager _singareasManager;
        private readonly SongsManager _songsManager;

        public SingersController(KtvSystemDBContext context, SingersManager singersManager, SingareasManager singareasManager, SongsManager songsManager)
        {
            _context = context;
            _singersManager = singersManager;
            _singareasManager = singareasManager;
            _songsManager = songsManager;
        }

        // GET: api/values
        [HttpGet]
        public string Get()
        {
            Console.WriteLine("Get all singers.");

            StateOfRequest mState = new StateOfRequest("SingNo");

            List<Singer> singers = _singersManager.GetAllSingers(mState);

            JObject jObjectForAll = new JObject();
            jObjectForAll.Add("pageNo", mState.CurrentPageNo);
            jObjectForAll.Add("pageSize", mState.PageSize);
            jObjectForAll.Add("totalRecords", mState.TotalRecords);
            jObjectForAll.Add("totalPages", mState.TotalPages);
            JObject jObject;
            JArray jArray = new JArray();
            foreach (var singer in singers)
            {
                jObject = JsonUtil.ConvertSingerToJsongObject(singer);
                jArray.Add(jObject);
            }
            jObjectForAll.Add("singers", jArray);

            return jObjectForAll.ToString();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<string> Get(int id)
        {
            // get one singer
            Singer singer = await _singersManager.FindOneSingerById(id);
            JObject jObject = JsonUtil.ConvertSingerToJsongObject(singer);
            JObject returnJSON = new JObject();
            returnJSON.Add("singer", jObject);

            return returnJSON.ToString();
        }

        // GET api/values/10/1
        [HttpGet("{pageSize}/{pageNo}")]
        public string Get(int pageSize, int pageNo)
        {
            Console.WriteLine("HttpGet[\"{ pageSize}/{ pageNo}\")]");

            StateOfRequest mState = new StateOfRequest("");
            mState.PageSize = pageSize;
            mState.CurrentPageNo = pageNo;
            List<Singer> singers = _singersManager.GetOnePageOfSingers(mState);

            JObject jObjectForAll = new JObject();
            jObjectForAll.Add("pageNo", mState.CurrentPageNo);
            jObjectForAll.Add("pageSize", mState.PageSize);
            jObjectForAll.Add("totalRecords", mState.TotalRecords);
            jObjectForAll.Add("totalPages", mState.TotalPages);
            JObject jObject;
            JArray jArray = new JArray();
            foreach (var singer in singers)
            {
                jObject = JsonUtil.ConvertSingerToJsongObject(singer);
                jArray.Add(jObject);
            }
            jObjectForAll.Add("singers", jArray);

            return jObjectForAll.ToString();
        }

        // GET api/values/10/1/orderBy
        [HttpGet("{pageSize}/{pageNo}/{orderBy}")]
        public string Get(int pageSize, int pageNo, string orderBy) {
            Console.WriteLine("HttpGet[\"{ pageSize}/{ pageNo}/{orderBy}\")]");

            // orderBy is either "SingNo" or "SingNa"

            string orderByParam = orderBy.Trim();
            if (string.IsNullOrEmpty(orderBy))
            {
                orderByParam = "";
            }
            else
            {
                orderByParam = orderBy.Trim();
            }


            StateOfRequest mState = new StateOfRequest(orderByParam);
            mState.PageSize = pageSize;
            mState.CurrentPageNo = pageNo;
            List<Singer> singers = _singersManager.GetOnePageOfSingers(mState);

            JObject jObjectForAll = new JObject();
            jObjectForAll.Add("pageNo", mState.CurrentPageNo);
            jObjectForAll.Add("pageSize", mState.PageSize);
            jObjectForAll.Add("totalRecords", mState.TotalRecords);
            jObjectForAll.Add("totalPages", mState.TotalPages);
            JObject jObject;
            JArray jArray = new JArray();
            foreach (var singer in singers)
            {
                jObject = JsonUtil.ConvertSingerToJsongObject(singer);
                jArray.Add(jObject);
            }
            jObjectForAll.Add("singers", jArray);

            return jObjectForAll.ToString();
        }

        // GET api/values/5/songs
        // [Route("{id}/[Action]")]
        // [HttpGet]
        // or
        [HttpGet("{id}/[Action]")]
        public string Songs(int id)
        {
            Console.WriteLine("HttpGet[\"{id}/Songs\")]");

            // pageSize = 1 does not matter
            // pageNo = -100 for all songs
            // orderBy = ""
            JObject jObjectForAll = GetSongsBySingerId(id, 1, -100, "", "");

            return jObjectForAll.ToString();
        }

        // GET api/values/5/songs/10/1
        // [Route("{id}/[Action]/{pageSize}/{pageNo}")]
        // [HttpGet]
        // or
        [HttpGet("{id}/[Action]/{pageSize}/{pageNo}")]
        public string Songs(int id, int pageSize, int pageNo)
        {
            Console.WriteLine("HttpGet[\"{id}/Songs/{ pageSize}/{ pageNo}\")]");

            // orderBy = ""
            JObject jObjectForAll = GetSongsBySingerId(id, pageSize, pageNo, "", "");

            return jObjectForAll.ToString();
        }

        // GET: api/values/5/songs/10/1/"SongNa"
        // [Route("{id}/[Action]/{pageSize}/{pageNo}/{orderBy}")]
        // [HttpGet]
        // or
        [HttpGet("{id}/[Action]/{pageSize}/{pageNo}/{orderBy}")]
        public string Songs(int id, int pageSize, int pageNo, string orderBy)
        {
            Console.WriteLine("HttpGet[\"{id}/Songs/{ pageSize}/{ pageNo}/{orderBy}\")]");

            JObject jObjectForAll = GetSongsBySingerId(id, pageSize, pageNo, orderBy, "");

            return jObjectForAll.ToString();
        }

        // GET: api/values/5/songs/10/1/"SongNa"/"SongNa+花心"
        // [Route("{id}/[Action]/{pageSize}/{pageNo}/{orderBy}/{filter}")]
        // [HttpGet]
        // or
        [HttpGet("{id}/[Action]/{pageSize}/{pageNo}/{orderBy}/{filter}")]
        public string Songs(int id, int pageSize, int pageNo, string orderBy, string filter)
        {
            Console.WriteLine("HttpGet[\"{id}/Songs/{ pageSize}/{ pageNo}/{orderBy}/{filter}\")]");

            JObject jObjectForAll = GetSongsBySingerId(id, pageSize, pageNo, orderBy, filter);

            return jObjectForAll.ToString();
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

        private JObject GetSongsBySingerId(int id, int pageSize, int pageNo, string orderBy, string filter)
        {
            string orderByParam;
            if (string.IsNullOrEmpty(orderBy))
            {
                orderByParam = "";
            }
            else
            {
                orderByParam = orderBy.Trim();
            }
            string filterParam;
            if (string.IsNullOrEmpty(filter))
            {
                filterParam = "";
            }
            else
            {
                filterParam = filter.Trim();
            }

            StateOfRequest mState = new StateOfRequest(orderByParam);
            mState.PageSize = pageSize;
            mState.CurrentPageNo = pageNo;
            mState.QueryCondition = filterParam;

            List<Song> songs = _songsManager.GetOnePageOfSongsBySingerId(mState, id, true);

            JObject jObjectForAll = new JObject();
            jObjectForAll.Add("pageNo", mState.CurrentPageNo);
            jObjectForAll.Add("pageSize", mState.PageSize);
            jObjectForAll.Add("totalRecords", mState.TotalRecords);
            jObjectForAll.Add("totalPages", mState.TotalPages);
            JObject jObject;
            JArray jArray = new JArray();
            foreach (var song in songs)
            {
                jObject = JsonUtil.ConvertSongToJsongObject(song);
                jArray.Add(jObject);
            }
            jObjectForAll.Add("songs", jArray);

            return jObjectForAll;
        }
    }
}
