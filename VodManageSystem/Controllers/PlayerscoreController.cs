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

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VodManageSystem.Controllers
{
    public class PlayerscoreController : Controller
    {
        private readonly PlayerscoreManager _playerscoreManager;

        public PlayerscoreController(PlayerscoreManager playerscoreManager) {
            _playerscoreManager = playerscoreManager;
        }

        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) {
                ViewData["Message"] = "Please login before doing data management.";
                return View();
            }
            int gameId = 1; // for Color balls
            List<Playerscore> top10List = await _playerscoreManager.GetTop10ScoresList(gameId);
            string rValue = JsonUtil.SetJsonStringFromObject(top10List);
            ViewData["Message"] = rValue;

            // ViewData["Message"] = "Under construction now ..........";
            return View();
        }

        /// <summary>
        /// Gets the top10 playerscores rest web.
        /// transfer data to Android App (5 Color Balls
        /// </summary>
        /// <returns>The top10 playerscores. return value is a JSON array</returns>
        [HttpGet]
        public async Task<List<Playerscore>> GetTop10PlayerscoresREST(int? gameId) {
            int gId = 1;
            if (gameId != null)
            {
                gId = gameId.Value;
            }

            List<Playerscore> top10List = await _playerscoreManager.GetTop10ScoresList(gId);

            return top10List;
        }

        /// <summary>
        /// Adds the one record rest web.
        /// receive data from Android App (5 Color Balls)
        /// </summary>
        /// <returns>yn means if succeeded then return "1". If failed then return "0"</returns>
        /// <param name="PlayerscoreJSON">Json string.</param>
        [HttpPost]
        public async Task<string> AddOneRecordREST(string PlayerscoreJSON) {
            string yn = "0";    // default is failed
            if ( String.IsNullOrEmpty(PlayerscoreJSON) ) {
                return yn;
            }
            try
            {
                JObject json = JObject.Parse(PlayerscoreJSON);
                Playerscore playerscore = new Playerscore();
                playerscore.PlayerName = (string)json["PlayerName"];
                playerscore.Score = Convert.ToInt32((string)json["Score"]);
                playerscore.GameId = Convert.ToInt32((string)json["GameId"]);
           
                int result = await _playerscoreManager.AddOnePlayerscoreToTable(playerscore);
                if (result == ErrorCodeModel.Succeeded)
                {
                    yn = "1";
                }
            } catch (Exception ex) {
                string msg = ex.StackTrace.ToString();
                Console.WriteLine("AddOneRecordREST failed:\n" + msg);
            }

            return yn;
        }
    }
}
