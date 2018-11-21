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
    public class SingerController : Controller
    {
        private readonly KtvSystemDBContext _context;
        private readonly SingerManager _singerManager;
        private readonly SingareaManager _singareaManager;
        private readonly SongManager _songManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:VodManageSystem.Controllers.SingerController"/> class.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="singerManager">Singer manager.</param>
        /// <param name="singareaManager">Singer Area manager.</param>
        public SingerController(KtvSystemDBContext context, SingerManager singerManager, SingareaManager singareaManager, SongManager songManager)
        {
            _context = context;
            _singerManager = singerManager;
            _singareaManager = singareaManager;
            _songManager = songManager;
        }

        // GET: api/values
        [HttpGet]
        public async Task<string> Get()
        {
            Console.WriteLine("Get all singers.");

            StateOfRequest mState = new StateOfRequest("SingNo");

            List<Singer> singers = await _singerManager.GetAllSingers(mState);

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
            Singer singer = await _singerManager.FindOneSingerById(id);
            JObject jObject = JsonUtil.ConvertSingerToJsongObject(singer);
            JObject returnJSON = new JObject();
            returnJSON.Add("singer", jObject);

            return returnJSON.ToString();
        }

        // GET api/values/10/1
        [HttpGet("{pageSize}/{pageNo}")]
        public async Task<string> Get(int pageSize, int pageNo)
        {
            Console.WriteLine("HttpGet[\"{ pageSize}/{ pageNo}\")]");

            StateOfRequest mState = new StateOfRequest("");
            mState.PageSize = pageSize;
            mState.CurrentPageNo = pageNo;
            // mState.OrderBy = orderByParam;
            List<Singer> singers = await _singerManager.GetOnePageOfSingersDictionary(mState);

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
        public async Task<string> Get(int pageSize, int pageNo, string orderBy) {
            Console.WriteLine("HttpGet[\"{ pageSize}/{ pageNo}/{orderBy}\")]");

            // orderBy is either "SingNo" or "SingNa"
            string orderByParam;
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
                orderByParam = "ReturnEmptyList";  // has to return empty list
            }

            StateOfRequest mState = new StateOfRequest(orderByParam);
            mState.PageSize = pageSize;
            mState.CurrentPageNo = pageNo;
            // mState.OrderBy = orderByParam;
            List<Singer> singers = await _singerManager.GetOnePageOfSingersDictionary(mState);

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

        // GET api/values/{id}/songs/10/1

        // [Route("{id}/[Action]/{pageSize}/{pageNo}")]
        // [HttpGet]
        // or
        [HttpGet("{id}/[Action]/{pageSize}/{pageNo}")]
        public string Songs(int id, int pageSize, int pageNo)
        {
            Console.WriteLine("HttpGet[\"{id}/Songs/{ pageSize}/{ pageNo}\")]");

            StateOfRequest mState = new StateOfRequest("");
            mState.PageSize = pageSize;
            mState.CurrentPageNo = pageNo;

            List<Song> songs = _songManager.GetOnePageOfSongsBySingerId(mState, id, true);

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

            return jObjectForAll.ToString();
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
                string orderByTemp = orderBy.ToUpper().Trim();
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

            StateOfRequest mState = new StateOfRequest(orderByParam);
            mState.PageSize = pageSize;
            mState.CurrentPageNo = pageNo;

            List<Singer> singers = await _singerManager.GetOnePageOfSingersByAreaSex(mState, areaId, sex);

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
