using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using VodManageSystem;
using VodManageSystem.Models;

namespace VodManageSystem.Controllers
{
    // docker.for.mac.localhost ----> for docker container
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Message"] = "Company description";
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
