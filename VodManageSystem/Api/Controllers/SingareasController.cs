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
    public class SingareasController : Controller
    {
        private readonly KtvSystemDBContext _context;
        private readonly SingareaManager _singareaManager;
        private readonly SingerManager _singerManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:VodManageSystem.Controllers.SingareaController"/> class.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="singareaManager">Singarea manager.</param>
        public SingareasController(KtvSystemDBContext context, SingareaManager singareaManager, SingerManager singerManager)
        {
            _context = context;
            _singareaManager = singareaManager;
            _singerManager = singerManager;

        }

        // GET: api/values
        [HttpGet]
        public string Get()
        {
            // get all singarea
            StateOfRequest mState = new StateOfRequest("AreaNo");
            List<Singarea> singareas = _singareaManager.GetAllSingareas(mState);

            // Convert List<Singarea> to JSON array
            JObject jObjectForAll = new JObject();
            jObjectForAll.Add("pageNo", mState.CurrentPageNo);
            jObjectForAll.Add("pageSize", mState.PageSize);
            jObjectForAll.Add("totalRecords", mState.TotalRecords);
            jObjectForAll.Add("totalPages", mState.TotalPages);
            JObject jObject;
            JArray jArray = new JArray();
            foreach (var singarea in singareas)
            {
                jObject = JsonUtil.ConvertSingareaToJsongObject(singarea);
                jArray.Add(jObject);
            }
            jObjectForAll.Add("singareas", jArray);

            return jObjectForAll.ToString();
        }

        // GET: api/SingerTypes

        // [Route("[Action]")]
        // [HttpGet]
        // or
        [HttpGet("[Action]")]
        public string SingerTypes()
        {
            // get all singarea
            StateOfRequest mState = new StateOfRequest("AreaNo");
            List<Singarea> singareas = _singareaManager.GetAllSingareas(mState);
            // Convert List<Singarea> to JSON array
            JObject jObjectForAll = new JObject();
            jObjectForAll.Add("pageNo", mState.CurrentPageNo);
            jObjectForAll.Add("pageSize", mState.PageSize);
            jObjectForAll.Add("totalRecords", mState.TotalRecords);
            jObjectForAll.Add("totalPages", mState.TotalPages);
            JObject jObject;
            JArray jArray = new JArray();
            foreach (var singarea in singareas)
            {
                jObject = JsonUtil.ConvertSingerTypeToJsongObject(singarea, "0");
                jArray.Add(jObject);
                jObject = JsonUtil.ConvertSingerTypeToJsongObject(singarea, "1");
                jArray.Add(jObject);
                jObject = JsonUtil.ConvertSingerTypeToJsongObject(singarea, "2");
                jArray.Add(jObject);
            }
            jObjectForAll.Add("singerTypes", jArray);

            return jObjectForAll.ToString();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<string> Get(int id)
        {
            // get one Singarea
            Singarea singarea = await _singareaManager.FindOneSingareaById(id);
            JObject jObject = JsonUtil.ConvertSingareaToJsongObject(singarea);

            JObject returnJSON = new JObject();
            returnJSON.Add("singarea", jObject);

            return returnJSON.ToString();
        }

        // Get api/values/5/Singers/1/10/1/"SingNa"

        // [Route("{id}/[Action]/{sex}/{pageSize}/{pageNo}/{orderBy}")]
        // [HttpGet]
        // or
        [HttpGet("{id}/[Action]/{sex}/{pageSize}/{pageNo}/{orderBy}")]
        public string Singers(int id, String sex, int pageSize, int pageNo, string orderBy)
        {
            Console.WriteLine("HttpGet[\"{id}/Singers/{sex}/{ pageSize}/{ pageNo}/{orderBy}\")]");
            Console.WriteLine("id = {0}, sex = {1}, pageSize = {2}, pageNo = {3}, orderBy = {4}", id, sex, pageSize, pageNo, orderBy);

            JObject jObjectForAll = GetSingers(id, sex, pageSize, pageNo, orderBy);

            return jObjectForAll.ToString();
        }

        // Get api/values/5/Singers/1/10/1

        // [Route("{id}/[Action]/{sex}/{pageSize}/{pageNo}")]
        // [HttpGet]
        // or
        [HttpGet("{id}/[Action]/{sex}/{pageSize}/{pageNo}")]
        public string Singers(int id, String sex, int pageSize, int pageNo)
        {
            Console.WriteLine("HttpGet[\"{id}/Singers/{sex}/{ pageSize}/{ pageNo}\")]");
            Console.WriteLine("id = {0}, sex = {1}, pageSize = {2}, pageNo = {3}", id, sex, pageSize, pageNo);

            JObject jObjectForAll = GetSingers(id, sex, pageSize, pageNo, "");

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

        private JObject GetSingers(int id, String sex, int pageSize, int pageNo, string orderBy)
        {
            // orderBy is either "", or "SingNo", or "SingNa"
            string orderByParam;
            if (string.IsNullOrEmpty(orderBy))
            {
                orderByParam = "";
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
                    orderByParam = "ReturnEmptyList";
                }
            }

            StateOfRequest mState = new StateOfRequest(orderByParam);
            mState.PageSize = pageSize;
            mState.CurrentPageNo = pageNo;

            List<Singer> singers = _singerManager.GetOnePageOfSingersByAreaSex(mState, id, sex, true);

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

            return jObjectForAll;
        }
    }
}
