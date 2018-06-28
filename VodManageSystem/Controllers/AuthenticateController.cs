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
using VodManageSystem.Utilities;
using Newtonsoft.Json.Linq;

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

        [HttpPost, ActionName("LoginForRESTfulService")]         // [ValidateAntiForgeryToken]   // prevent forgery         // the following method definition is wrong for REST Web Service         // public async Task<IActionResult> LoginFromAndroid(string username, string password)         public async Task LoginForRESTfulService(string username, string password)         {             string uName = string.Empty;             string pWord = string.Empty;

            bool connectedToDatabase = true;             User user = new User();             if ((!string.IsNullOrEmpty(username)) && (!string.IsNullOrEmpty(password)))             {                 uName = username.Trim();                 pWord = password.Trim();
                try {                     user = await _context.User.SingleOrDefaultAsync(m => (m.UserName == uName) && m.UserPassword == pWord);                     if (user == null)                     {
                        // the user not found
                        user.UserName = "User not found or wrong password.";                         user.UserEmail = "";                         user.UserState = "";                     }
                }
                catch (Exception ex)
                {
                    string msg = ex.StackTrace.ToString();
                    Console.WriteLine(msg);
                    // jsonObject.Property("isConnectedToJDBC").Value = false;
                    connectedToDatabase = false;
                    user.UserName = "Database Exception happend.";
                    user.UserEmail = "";
                    user.UserState = "";
                }             }             else             {                 // empty user name of password                 user.UserName = "User name can not be blank.";                 user.UserEmail = "";                 user.UserState = "";             }              // string userJSON = JsonUtil.SetJsonStringFromObject(user);
            // JObject jsonObject = JObject.Parse(userJSON);
            // jsonObject.Add("isConnectedToJDBC", connectedToDatabase);
            // the following 2 statements are also set value to a property of JObject object
            // jsonObject.Property("IsConnectedToJDBC").Value = connectedToDatabase;
            // jsonObject["IsConnectedToJDBC"] = connectedToDatabase;

            JObject jsonObject = new JObject();
            jsonObject.Add("isConnectedToJDBC", connectedToDatabase);
            jsonObject.Add("userName", user.UserName);
            jsonObject.Add("userEmail", user.UserEmail);
            jsonObject.Add("userState", user.UserState);

            // Response.ContentType = "text/plain; charset=utf-8";
            Response.ContentType = "application/json; charset=utf-8";             await Response.WriteAsync(jsonObject.ToString(), Encoding.UTF8);              // or             // byte[] mBytes = Encoding.UTF8.GetBytes(jsonObject.ToString());             // await Response.Body.WriteAsync(mBytes,0,mBytes.Length);         } 

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
