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
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VodManageSystem.Api.Controllers
{
    [Route("api/[controller]")]
    public class LanguagesController : Controller
    {
        private readonly KtvSystemDBContext _context;
        private readonly LanguagesManager _languagesManager;
        private readonly SongsManager _songsManager;

        public LanguagesController(KtvSystemDBContext context, LanguagesManager languagesManager, SongsManager songsManager)
        {
            _context = context;
            _languagesManager = languagesManager;
            _songsManager = songsManager;
        }

        // GET: api/values
        [HttpGet]
        public string Get()
        {
            // get all singarea
            StateOfRequest mState = new StateOfRequest("LangNo");
            List<Language> languages = _languagesManager.GetAllLanguages(mState);

            // Convert List<Language> to JSON array
            JObject jObjectForAll = new JObject();
            jObjectForAll.Add("pageNo", mState.CurrentPageNo);
            jObjectForAll.Add("pageSize", mState.PageSize);
            jObjectForAll.Add("totalRecords", mState.TotalRecords);
            jObjectForAll.Add("totalPages", mState.TotalPages);
            JObject jObject;
            JArray jArray = new JArray();
            foreach (var language in languages)
            {
                jObject = JsonUtil.ConvertlanguageToJsongObject(language);
                jArray.Add(jObject);
            }
            jObjectForAll.Add("languages", jArray);

            return jObjectForAll.ToString();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<string> Get(int id)
        {
            // get one language
            Language language = await _languagesManager.FindOneLanguageById(id);
            JObject jObject = JsonUtil.ConvertlanguageToJsongObject(language);

            JObject returnJSON = new JObject();
            returnJSON.Add("language", jObject);

            return returnJSON.ToString();
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
            JObject jObjectForAll = GetSongsByLanguageId(id, 1, -100, "");

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
            JObject jObjectForAll = GetSongsByLanguageId(id, pageSize, pageNo, "");

            return jObjectForAll.ToString();
        }

        // GET: api/values/5/Songs/10/1/"SongNa"
        // [Route("{id}/[Action]/{pageSize}/{pageNo}/{orderBy}")]
        // [HttpGet]
        // or
        [HttpGet("{id}/[Action]/{pageSize}/{pageNo}/{orderBy}")]
        public string Songs(int id, int pageSize, int pageNo, string orderBy)
        {
            Console.WriteLine("HttpGet[\"{id}/Songs/{ pageSize}/{ pageNo}/{orderBy}\")]");

            JObject jObjectForAll = GetSongsByLanguageId(id, pageSize, pageNo, orderBy);

            Console.WriteLine(jObjectForAll);

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

        private JObject GetSongsByLanguageId(int id, int pageSize, int pageNo, string orderBy)
        {
            string orderByParam;
            if (string.IsNullOrEmpty(orderBy))
            {
                orderByParam = "";
            }
            else
            {
                string orderByTemp = orderBy.ToUpper().Trim();
                if (orderByTemp == "SONGNO")
                {
                    orderByParam = "SongNo";
                }
                else if (orderByTemp == "SONGNA")
                {
                    orderByParam = "SongNa";
                }
                else if (orderByTemp == "VODNO")
                {
                    orderByParam = "VodNo";
                }
                else if (orderBy == "LANG_SONGNA")
                {
                    orderByParam = "LangSongNa";
                }
                else if (orderBy == "SINGER1_NA")
                {
                    orderByParam = "Singer1Na";
                }
                else if (orderBy == "SINGER1_NA")
                {
                    orderByParam = "Singer2Na";
                }
                else
                {
                    orderByParam = "ReturnEmptyList";  // has to return empty list
                }
            }

            StateOfRequest mState = new StateOfRequest(orderByParam);
            mState.PageSize = pageSize;
            mState.CurrentPageNo = pageNo;

            List<Song> songs = _songsManager.GetOnePageOfSongsByLanguageId(mState, id, true);

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
