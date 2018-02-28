using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VodManageSystem.Models;
using VodManageSystem.Models.Dao;
using VodManageSystem.Models.DataModels;
using VodManageSystem.Utilities;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VodManageSystem.Controllers
{
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

        // GET: /<controller>/
        public IActionResult Index(string singarea_state)
        {
            // new Index.cshtml
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View();

            SingareaStateOfRequest singareaState = JsonUtil.GetObjectFromJsonString<SingareaStateOfRequest>(singarea_state);
            string temp_state = JsonUtil.SetJsonStringFromObject(singareaState);
            // go to songs management main menu
            return RedirectToAction(nameof(SingareasList), new { singarea_state = temp_state });    // (action, parameters)
        }

        // Get: get method
        [HttpGet, ActionName("SingareasList")]
        public async Task<IActionResult> SingareasList(string singarea_state)
        {
            SingareaStateOfRequest singareaState = JsonUtil.GetObjectFromJsonString<SingareaStateOfRequest>(singarea_state);
            List<Singarea> singareas = await _singareaManager.GetOnePageOfSingareasDictionary(singareaState);
            ViewBag.SingareaState = JsonUtil.SetJsonStringFromObject(singareaState);

            return View(singareas);
        }
    }
}
