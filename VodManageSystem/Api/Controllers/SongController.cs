﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using VodManageSystem.Models;
using VodManageSystem.Models.Dao;
using VodManageSystem.Models.DataModels;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VodManageSystem.Api.Controllers
{
    [Route("api/[controller]")]
    public class SongController : Controller
    {
        private readonly KtvSystemDBContext _context;
        private readonly SongManager _songManager;
        private readonly LanguageManager _languageManager;
        private readonly SingerManager _singerManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:VodManageSystem.Controllers.SongController"/> class.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="songManager">Song manager.</param>
        /// <param name="languagemanager">Languagemanager.</param>
        /// <param name="singerManager">Singer manager.</param>
        public SongController(KtvSystemDBContext context, SongManager songManager, LanguageManager languagemanager, SingerManager singerManager)
        {
            _context = context;
            _songManager = songManager;
            _languageManager = languagemanager;
            _singerManager = singerManager;
        }

        // GET: api/values
        [HttpGet]
        public async Task<string> Get()
        {
            Console.WriteLine("Get all songs.");

            StateOfRequest mState = new StateOfRequest("SongNo");
            mState.CurrentPageNo = 1;
            mState.PageSize = 50;

            // List<Song> songs = await _songManager.GetAllSongs(mState);
            // get the first 50 songs
            List<Song> songs = await _songManager.GetOnePageOfSongs(mState);

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
                jObject = ConvertSongToJsongObject(song);
                jArray.Add(jObject);
            }
            jObjectForAll.Add("songs", jArray);

            return jObjectForAll.ToString();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
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

        private JObject ConvertSongToJsongObject(Song song)
        {
            JObject jObject = new JObject();
            if (song == null)
            {
                return jObject;
            }

            jObject.Add("id", song.Id);
            jObject.Add("songNo", song.SongNo);
            jObject.Add("songNa", song.SongNa);
            jObject.Add("languageId", song.LanguageId);
            jObject.Add("sNumWord", song.SNumWord);
            jObject.Add("numFw", song.NumFw);
            jObject.Add("numPw", song.NumPw);
            jObject.Add("chor", song.Chor);
            jObject.Add("nMpeg", song.NMpeg);
            jObject.Add("mMpeg", song.MMpeg);
            jObject.Add("vodYn", song.VodYn);
            jObject.Add("vodNo", song.VodNo);
            jObject.Add("pathname", song.Pathname);
            jObject.Add("ordNo", song.OrdNo);
            jObject.Add("orderNum", song.OrderNum);
            jObject.Add("ordOldN", song.OrdOldN);
            jObject.Add("languageId", song.LanguageId);
            if (song.Language != null)
            {
                jObject.Add("langNo", song.Language.LangNo);
                jObject.Add("langNa", song.Language.LangNa);
            }
            else 
            {
                jObject.Add("langNa", "");
            }
            jObject.Add("singer1Id", song.Singer1Id);
            if (song.Singer1 != null)
            {
                jObject.Add("singer1No", song.Singer1.SingNo);
                jObject.Add("singer1Na", song.Singer1.SingNa);
            }
            else 
            {
                jObject.Add("singer1No", "");
                jObject.Add("singer1Na", "");
            }
            jObject.Add("singer2Id", song.Singer2Id);
            if (song.Singer2 != null)
            {
                jObject.Add("singer2No", song.Singer2.SingNo);
                jObject.Add("singer2Na", song.Singer2.SingNa);
            }
            else
            {
                jObject.Add("singer2No", "");
                jObject.Add("singer2Na", "");
            }

            jObject.Add("inDate", song.InDate);

            return jObject;
        }
    }
}