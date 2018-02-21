using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Html;

using VodManageSystem.Models.DataModels;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VodManageSystem.Controllers
{
    public class DatabaseController : Controller
    {
        private readonly KtvSystemDBContext _context;

        public DatabaseController(KtvSystemDBContext context)
        {
            _context = context;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            ISession session = HttpContext.Session;
            if (session.GetInt32("LoggedIn") == 1)
            {
                // go to data management main menu
                return RedirectToAction("DataManageMenu");    // (action, controller)
            }
            else
            {
                ViewData["Message"] = "Please login before doing data management.";
                return View();
            }
        }

        // GET:
        [HttpGet, ActionName("DataManageMenu")]
        public IActionResult DataManageMenu()
        {
            // go to Song management
            // return RedirectToAction("Index", "Song");    // (action, controller)
            // or                                
            // return RedirectToRoute(new { controller = "Song", Action = "Index" });                                           
            return View();
        }
    }
}
