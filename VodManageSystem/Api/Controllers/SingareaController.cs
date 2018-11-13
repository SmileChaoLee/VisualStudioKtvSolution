using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VodManageSystem.Models;
using VodManageSystem.Models.Dao;
using VodManageSystem.Models.DataModels;

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
        public async Task<IEnumerable<Singarea>> Get()
        {
            SingareaStateOfRequest singareaState = new SingareaStateOfRequest();
            return await _singareaManager.GetOnePageOfSingareasDictionary(singareaState);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<Singarea> Get(int id)
        {
            Console.WriteLine("id = " + id);
            Singarea sArea = await _singareaManager.FindOneSingareaById(id);
            if (sArea == null) {
                Console.WriteLine("Did not find a Singarea record.");
            }
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
