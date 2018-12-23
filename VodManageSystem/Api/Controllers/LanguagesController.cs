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
        private const int Normal_SongType = 0;
        private const int NewSong_SongType = 1;
        private const int HotSong_SongType = 2;

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
            JObject jObjectForAll = GetSongsByLanguageIdAndSomething(id, -1, 1, -100, "", Normal_SongType, "");

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
            JObject jObjectForAll = GetSongsByLanguageIdAndSomething(id, -1, pageSize, pageNo, "", Normal_SongType, "");

            return jObjectForAll.ToString();
        }

        // GET api/values/5/songs/10/1/"SongNa+花心"
        // [Route("{id}/[Action]/{pageSize}/{pageNo}/{filter}")]
        // [HttpGet]
        // or
        [HttpGet("{id}/[Action]/{pageSize}/{pageNo}/{filter}")]
        public string Songs(int id, int pageSize, int pageNo, string filter)
        {
            Console.WriteLine("HttpGet[\"{id}/Songs/{ pageSize}/{ pageNo}/{silter}\")]");

            // orderBy = ""
            JObject jObjectForAll = GetSongsByLanguageIdAndSomething(id, -1, pageSize, pageNo, "", Normal_SongType, filter);

            return jObjectForAll.ToString();
        }

        // GET: api/values/5/Songs/10/1/"SongNa"
        // [Route("{id}/[Action]/{pageSize}/{pageNo}/{orderBy}")]
        // [HttpGet]
        // or
        [HttpGet("{id}/[Action]/{pageSize}/{pageNo}/{orderBy}")]
        public string SongsOrderBy(int id, int pageSize, int pageNo, string orderBy)
        {
            Console.WriteLine("HttpGet[\"{id}/Songs/{ pageSize}/{ pageNo}/{orderBy}\")]");

            JObject jObjectForAll = GetSongsByLanguageIdAndSomething(id, -1, pageSize, pageNo, orderBy, Normal_SongType, "");

            return jObjectForAll.ToString();
        }

        // GET: api/values/5/Songs/10/1/"SongNa"/"SongNa+花心"
        // [Route("{id}/[Action]/{pageSize}/{pageNo}/{orderBy}/{filter}")]
        // [HttpGet]
        // or
        [HttpGet("{id}/[Action]/{pageSize}/{pageNo}/{orderBy}/{filter}")]
        public string SongsOrderBy(int id, int pageSize, int pageNo, string orderBy, string filter)
        {
            Console.WriteLine("HttpGet[\"{id}/Songs/{ pageSize}/{ pageNo}/{orderBy}/{filter}\")]");

            JObject jObjectForAll = GetSongsByLanguageIdAndSomething(id, -1, pageSize, pageNo, orderBy, Normal_SongType, filter);

            return jObjectForAll.ToString();
        }

        // GET: api/values/5/7/Songs/10/1/"SongNa"
        // [Route("{id}/{numOfWords}/[Action]/{pageSize}/{pageNo}/{orderBy}")]
        // [HttpGet]
        // or
        [HttpGet("{id}/{numOfWords}/[Action]/{pageSize}/{pageNo}/{orderBy}")]
        public string Songs(int id, int numOfWords, int pageSize, int pageNo, string orderBy)
        {
            Console.WriteLine("HttpGet[\"{id}/{numOfWords}/Songs/{ pageSize}/{ pageNo}/{orderBy}\")]");
            Console.WriteLine("\n\n numOfWrods == " + numOfWords + "\n\n");

            JObject jObjectForAll = GetSongsByLanguageIdAndSomething(id, numOfWords, pageSize, pageNo, orderBy, Normal_SongType, "");

            return jObjectForAll.ToString();
        }

        // GET: api/values/5/7/Songs/10/1/"SongNa"/"SongNa+花心"
        // [Route("{id}/{numOfWords}/[Action]/{pageSize}/{pageNo}/{orderBy}/{filter}")]
        // [HttpGet]
        // or
        [HttpGet("{id}/{numOfWords}/[Action]/{pageSize}/{pageNo}/{orderBy}/{filter}")]
        public string Songs(int id, int numOfWords, int pageSize, int pageNo, string orderBy, string filter)
        {
            Console.WriteLine("HttpGet[\"{id}/{numOfWords}/Songs/{ pageSize}/{ pageNo}/{orderBy}/{filter}\")]");
            Console.WriteLine("\n\n numOfWrods == " + numOfWords + "\n\n");

            JObject jObjectForAll = GetSongsByLanguageIdAndSomething(id, numOfWords, pageSize, pageNo, orderBy, Normal_SongType, filter);

            return jObjectForAll.ToString();
        }

        // GET: api/values/5/NewSongs/10/1
        // [Route("{id}/[Action]/{pageSize}/{pageNo}")]
        // [HttpGet]
        // or
        [HttpGet("{id}/[Action]/{pageSize}/{pageNo}")]
        public string NewSongs(int id, int pageSize, int pageNo)
        {
            Console.WriteLine("HttpGet[\"{id}/NewSongs/{ pageSize}/{ pageNo}\")]");

            JObject jObjectForAll = GetSongsByLanguageIdAndSomething(id, 0, pageSize, pageNo, "", NewSong_SongType, "");

            return jObjectForAll.ToString();
        }

        // GET: api/values/5/NewSongs/10/1/"SongNa+花心"
        // [Route("{id}/[Action]/{pageSize}/{pageNo}/{filter}")]
        // [HttpGet]
        // or
        [HttpGet("{id}/[Action]/{pageSize}/{pageNo}/{filter}")]
        public string NewSongs(int id, int pageSize, int pageNo, string filter)
        {
            Console.WriteLine("HttpGet[\"{id}/NewSongs/{ pageSize}/{ pageNo}/{filter}\")]");

            JObject jObjectForAll = GetSongsByLanguageIdAndSomething(id, 0, pageSize, pageNo, "", NewSong_SongType, filter);

            return jObjectForAll.ToString();
        }

        // GET: api/values/5/HotSongs/10/1
        // [Route("{id}/[Action]/{pageSize}/{pageNo}")]
        // [HttpGet]
        // or
        [HttpGet("{id}/[Action]/{pageSize}/{pageNo}")]
        public string HotSongs(int id, int pageSize, int pageNo)
        {
            Console.WriteLine("HttpGet[\"{id}/HotSongs/{ pageSize}/{ pageNo}\")]");

            JObject jObjectForAll = GetSongsByLanguageIdAndSomething(id, 0, pageSize, pageNo, "", HotSong_SongType, "");

            return jObjectForAll.ToString();
        }

        // GET: api/values/5/HotSongs/10/1/"SongNa+花心"
        // [Route("{id}/[Action]/{pageSize}/{pageNo}/{filter}")]
        // [HttpGet]
        // or
        [HttpGet("{id}/[Action]/{pageSize}/{pageNo}/{filter}")]
        public string HotSongs(int id, int pageSize, int pageNo, string filter)
        {
            Console.WriteLine("HttpGet[\"{id}/HotSongs/{ pageSize}/{ pageNo}/{filter}\")]");

            JObject jObjectForAll = GetSongsByLanguageIdAndSomething(id, 0, pageSize, pageNo, "", HotSong_SongType, filter);

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

        private JObject GetSongsByLanguageIdAndSomething(int id, int numOfWords, int pageSize, int pageNo, string orderBy, int songType, String filter)
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

            List<Song> songs = new List<Song>();
            switch (songType) {
                case Normal_SongType:
                    // normal
                    songs = _songsManager.GetOnePageOfSongsByLanguageIdNumOfWords(mState, id, numOfWords, true);
                    break;
                case NewSong_SongType:
                    // new songs
                    songs = _songsManager.GetOnePageOfNewSongByLanguageId(mState, id, true);
                    break;
                case HotSong_SongType:
                    // hot songs
                    songs = _songsManager.GetOnePageOfHotSongByLanguageId(mState, id, true);
                    break;
                default:
                    break;
            }

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
