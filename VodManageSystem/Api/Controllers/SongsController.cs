﻿using System;
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
    public class SongsController : Controller
    {
        private readonly KtvSystemDBContext _context;
        private readonly SongsManager _songManager;
        private readonly LanguagesManager _languageManager;
        private readonly SingersManager _singerManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:VodManageSystem.Controllers.SongController"/> class.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="songManager">Song manager.</param>
        /// <param name="languagemanager">Languagemanager.</param>
        /// <param name="singerManager">Singer manager.</param>
        public SongsController(KtvSystemDBContext context, SongsManager songManager, LanguagesManager languagemanager, SingersManager singerManager)
        {
            _context = context;
            _songManager = songManager;
            _languageManager = languagemanager;
            _singerManager = singerManager;
        }

        // GET: api/values
        [HttpGet]
        public string Get()
        {
            Console.WriteLine("Get all songs.");

            StateOfRequest mState = new StateOfRequest("SongNo");
            mState.CurrentPageNo = 1;
            mState.PageSize = 50;

            // List<Song> songs = await _songManager.GetAllSongs(mState);
            // get the first 50 songs
            List<Song> songs = _songManager.GetOnePageOfSongs(mState);

            JObject jObjectForAll = new JObject();
            jObjectForAll.Add("pageNo", mState.CurrentPageNo);
            jObjectForAll.Add("pageSize", mState.PageSize);
            jObjectForAll.Add("totalRecords", mState.TotalRecords);
            jObjectForAll.Add("totalPages", mState.TotalPages);
            JObject jObject;
            JArray jArray = new JArray();
            for (int i = 0; i < songs.Count; i++)
            {
                Song song = songs[i];
                jObject = JsonUtil.ConvertSongToJsongObject(song);
                jArray.Add(jObject);
            }
            jObjectForAll.Add("songs", jArray);

            return jObjectForAll.ToString();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<string> Get(int id)
        {
            // get one song
            Song song = await _songManager.FindOneSongById(id);
            JObject jObject = JsonUtil.ConvertSongToJsongObject(song);
            JObject returnJSON = new JObject();
            returnJSON.Add("song", jObject);

            return returnJSON.ToString();
        }

        // GET api/values/10/1
        [HttpGet("{pageSize}/{pageNo}")]
        public string Get(int pageSize, int pageNo)
        {
            Console.WriteLine("HttpGet[\"{ pageSize}/{ pageNo}\")]");

            JObject jObjectForAll = GetSongs(pageSize, pageNo, "");

            return jObjectForAll.ToString();
        }

        // GET api/values/10/1/"SongNo"
        [HttpGet("{pageSize}/{pageNo}/{orderBy}")]
        public string Get(int pageSize, int pageNo, string orderBy)
        {
            Console.WriteLine("HttpGet[\"{ pageSize}/{ pageNo}/{orderBy}\")]");

            JObject jObjectForAll = GetSongs(pageSize, pageNo, orderBy);

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

        private JObject GetSongs(int pageSize, int pageNo, string orderBy)
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
            List<Song> songs = _songManager.GetOnePageOfSongs(mState);

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
