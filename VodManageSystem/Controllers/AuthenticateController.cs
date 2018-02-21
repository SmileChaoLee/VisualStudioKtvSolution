using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using VodManageSystem.Models.DataModels;
using System.Text;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VodManageSystem.Controllers
{
    public class AuthenticateController : Controller
    {
        private readonly KtvSystemDBContext _context;

        public AuthenticateController(KtvSystemDBContext context)
        {
            _context = context;
        }

        // GET: /<controller>/
        public IActionResult Login()
        {
            ISession session = HttpContext.Session;
            session.SetInt32("LoggedIn", 0); // have not logged in yet
            ViewData["ErrorMessage"] = "Please input user name and pasword.";
            return View();
        }

        [HttpPost, ActionName("Login")]
        [ValidateAntiForgeryToken]   // prevent forgery
        public async Task<IActionResult> CheckAuthentication(string username, string password)
        {
            if ((!string.IsNullOrEmpty(username)) && (!string.IsNullOrEmpty(password)))
            {
                username = username.Trim();
                password = password.Trim();
                var user = await _context.User.SingleOrDefaultAsync(m => (m.UserName == username) && m.UserPassword == password);
                if (user != null)
                {
                    // found the user
                    ISession session = HttpContext.Session;
                    session.SetInt32("LoggedIn", 1); // logged in
                    return RedirectToAction("LoggedIn");
                }
            }

            // user name not found
            string str = "User name were not found or wrong password.<br/>Please try again.";
            ViewData["ErrorMessage"] = new HtmlString(str);
            // user name not found or wrong password
            // return RedirectToAction(nameof(Index));
            return View();
        }

        [HttpPost, ActionName("LoginFromAndroid")]
        // [ValidateAntiForgeryToken]   // prevent forgery
        // the following method definition is wrong for REST Web Service
        // public async Task<IActionResult> LoginFromAndroid(string username, string password)
        public async Task LoginFromAndroid(string username, string password)
        {
            bool loginYn = false;
            if ((!string.IsNullOrEmpty(username)) && (!string.IsNullOrEmpty(password)))
            {
                username = username.Trim();
                password = password.Trim();
                var user = await _context.User.SingleOrDefaultAsync(m => (m.UserName == username) && m.UserPassword == password);
                if (user != null)
                {
                    // found the user
                    loginYn = true;
                }
            }

            String message;
            Response.ContentType = "text/plain";
            if (loginYn)
            {
                message = "Succeeded to login (test.)";
            }
            else
            {
                message = "Failed to login (test.)";
            }

            await Response.WriteAsync(message, Encoding.UTF8);

            // or
            // byte[] mBytes = Encoding.UTF8.GetBytes(message);
            // await Response.Body.WriteAsync(mBytes,0,mBytes.Length);
        }

        // GET: /<controller>/
        [HttpGet]
        public IActionResult LoggedIn()
        {
            ViewData["Message"] = "Logged in successfully.";
            return View();
        }

        // GET: /<controller>/
        [HttpGet]
        public IActionResult LoggedOut()
        {
            ISession session = HttpContext.Session;
            session.SetInt32("LoggedIn", 0); // logged out
            ViewData["Message"] = "Logged out successfully.";
            return View();
        }
    }
}
