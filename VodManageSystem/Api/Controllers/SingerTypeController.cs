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
    public class SingerTypeController : Controller
    {
        private readonly KtvSystemDBContext _context;
        private readonly SingareaManager _singareaManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:VodManageSystem.Controllers.SingareaController"/> class.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="singareaManager">Singarea manager.</param>
        public SingerTypeController(KtvSystemDBContext context, SingareaManager singareaManager)
        {
            _context = context;
            _singareaManager = singareaManager;
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
                jObject = ConvertSingareaToJsongObject(singarea, "0");
                jArray.Add(jObject);
                jObject = ConvertSingareaToJsongObject(singarea, "1");
                jArray.Add(jObject);
                jObject = ConvertSingareaToJsongObject(singarea, "2");
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
            JObject jObject = ConvertSingareaToJsongObject(singarea, "0");

            JObject returnJSON = new JObject();
            returnJSON.Add("singerType", jObject);

            return returnJSON.ToString();
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

        private JObject ConvertSingareaToJsongObject(Singarea singarea, string sex)
        {
            JObject jObject = new JObject();
            if (singarea == null)
            {
                return jObject;
            }

            jObject.Add("id", singarea.Id);
            jObject.Add("areaNo", singarea.AreaNo);
            jObject.Add("areaNa", singarea.AreaNa);
            jObject.Add("areaEn", singarea.AreaEn);
            jObject.Add("sex", sex);

            return jObject;
        }
    }
}
