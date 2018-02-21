using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VodManageSystem.Models;
using VodManageSystem.Models.Dao;
using VodManageSystem.Models.DataModels;
using VodManageSystem.Utilities;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VodManageSystem.Controllers
{
    public class LanguageController : Controller
    {
        private readonly KtvSystemDBContext _context;
        private readonly LanguageManager _languageManager;
        /// <summary>
        /// Initializes a new instance of the <see cref="T:VodManageSystem.Controllers.LanguageController"/> class.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="languageManager">Language manager.</param>
        public LanguageController(KtvSystemDBContext context, LanguageManager languageManager)
        {
            _context = context;
            _languageManager = languageManager;
        }

        // GET: /<controller>/
        public IActionResult Index(string language_state)
        {
            // new Index.cshtml
            ISession session = HttpContext.Session;
            if (session.GetInt32("LoggedIn") == 1)
            {
                /*
                LanguageStateOfRequest languageState = JsonUtil.GetObjectFromJsonString<LanguageStateOfRequest>(language_state);
                string temp_state = JsonUtil.SetJsonStringFromObject(languageState);
                // go to languages management main menu
                return RedirectToAction("LanguagesList", new { language_state = temp_state });    // (action, parameters)
                */

                // go to Languages management main menu
                return RedirectToAction("LanguagesList", new { language_state = language_state });    // (action, parameters)
            }
            else
            {
                ViewData["Message"] = "Please login before doing data management.";
                return View();
            }
        }

        // Get: get method
        [HttpGet, ActionName("LanguagesList")]
        public async Task<IActionResult> LanguagesList(string language_state)
        {
            LanguageStateOfRequest languageState = JsonUtil.GetObjectFromJsonString<LanguageStateOfRequest>(language_state);
            List<Language> languages = await _languageManager.GetOnePageOfLanguagesDictionary(languageState);
            ViewBag.LanguageState = JsonUtil.SetJsonStringFromObject(languageState);

            return View();

        }
    }
}
