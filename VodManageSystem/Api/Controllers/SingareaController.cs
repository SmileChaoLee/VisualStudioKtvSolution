using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VodManageSystem.Models;
using VodManageSystem.Models.Dao;
using VodManageSystem.Models.DataModels;
using VodManageSystem.Utilities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VodManageSystem.Api.Controllers
{
    [Route("api/[controller]")]
    public class SingareaController : Controller
    {
        private readonly KtvSystemDBContext _context;
        private readonly SingareaManager _singareaManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:VodManageSystem.Controllers.SingareaController"/> class.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="singareaManager">Singarea manager.</param>
        public SingareaController(KtvSystemDBContext context, SingareaManager singareaManager)
        {
            _context = context;
            _singareaManager = singareaManager;
        }

        // GET: api/values
        [HttpGet]
        public async Task<string> Get()
        {
            // get all singarea
            SingareaStateOfRequest singareaState = new SingareaStateOfRequest();
            List<Singarea> singareas = await _singareaManager.GetAllSingareasAsync(singareaState);
            // convert singers object to JSON string (serializing)
            string singareasString = JsonUtil.SetJsonStringFromObject(singareas);
            return singareasString;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<Singarea> Get(int id)
        {
            // get one Singarea
            Singarea sArea = await _singareaManager.FindOneSingareaById(id);
            return sArea;
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
