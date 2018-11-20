using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

using VodManageSystem.Utilities;
using VodManageSystem.Models;
using VodManageSystem.Models.Dao;
using VodManageSystem.Models.DataModels;
using Microsoft.AspNetCore.Http;

namespace VodManageSystem.Controllers
{
    /// <summary>
    /// Song controller.
    /// The controller uses TempData extension method to keep some data that it needs.
    /// Like current page no., Song.SongNo, OrderBy, and QueryCondition
    /// Use TempDate.Set() to add or reset data and use TempData.Peek() to have data
    /// but not to remove from TempData dictionary. If TempDate.Get() is used, data will be removed
    /// after Get() method.
    /// </summary>
    public class SongController : Controller
    {
        private readonly KtvSystemDBContext _context;
        private readonly SongManager _songManager;
        private readonly LanguageManager _languageManager;
        private readonly SingerManager _singerManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:VodManageSystem.Controllers.SongController"/> class.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="songManager">Song manager.</param>
        /// <param name="languagemanager">Languagemanager.</param>
        /// <param name="singerManager">Singer manager.</param>
        public SongController(KtvSystemDBContext context, SongManager songManager, LanguageManager languagemanager, SingerManager singerManager)
        {
            _context = context;
            _songManager = songManager;
            _languageManager = languagemanager;
            _singerManager = singerManager;
        }

        // GET: Song
        public IActionResult Index(string song_state)
        {
            // new Index.cshtml
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View();

            StateOfRequest mState;
            if (string.IsNullOrEmpty(song_state))
            {
                mState = new StateOfRequest("SongNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(song_state);
            }
            string temp_state = JsonUtil.SetJsonStringFromObject(mState);
            // go to songs management main menu
            return RedirectToAction(nameof(SongsList), new { song_state = temp_state });    // (action, parameters)
        }

        // get a list of songs
        // Get method.  
        [HttpGet, ActionName("SongsList")]
        public async Task<IActionResult> SongsList(string song_state)
        {
            StateOfRequest mState;
            if (string.IsNullOrEmpty(song_state))
            {
                mState = new StateOfRequest("SongNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(song_state);
            }
            // List<Song> songs = await _songManager.GetOnePageOfSongsDictionary(mState);
            List<Song> songs = await _songManager.GetOnePageOfSongs(mState);

            ViewBag.SongState = JsonUtil.SetJsonStringFromObject(mState);
            return View(songs);
        }

        // Get
        [HttpGet, ActionName("Find")]
        public async Task<IActionResult> Find(string song_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            List<SelectListItem> languageSelectList = await _languageManager.GetSelectListOfLanguages(new StateOfRequest("LangNa"));
            List<SelectListItem> singerSelectList = await _singerManager.GetSelectListOfSingers(new StateOfRequest("SingNa"));

            ViewBag.LanguageList = languageSelectList;
            ViewBag.SingerList = singerSelectList;
            ViewBag.SongState = song_state;
            return View();
        }

        // Post
        [HttpPost, ActionName("Find")]
        public async Task<IActionResult> Find(string song_no, string vod_no, string song_na, int languageId, string sing_na1, string sing_na2, string search_type, string submitbutton, string song_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            StateOfRequest mState;
            if (string.IsNullOrEmpty(song_state))
            {
                mState = new StateOfRequest("SongNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(song_state);
            }
            mState.StartTime = DateTime.Now;
            string temp_state = JsonUtil.SetJsonStringFromObject(mState);

            if (string.IsNullOrEmpty(song_no))
            {
                song_no = string.Empty;
            }
            song_no = song_no.Trim();

            if (string.IsNullOrEmpty(vod_no))
            {
                vod_no = string.Empty;
            }
            vod_no = vod_no.Trim();

            if (string.IsNullOrEmpty(song_na))
            {
                song_na = string.Empty;
            }
            song_na = song_na.Trim();

            string lang_no = "";
            if (languageId >= 0)
            {
                Language language = await _languageManager.FindOneLanguageById(languageId);
                if (language != null)
                {
                    lang_no = language.LangNo;
                }
            }

            if (string.IsNullOrEmpty(sing_na1))
            {
                sing_na1 = string.Empty;
            }
            sing_na1 = sing_na1.Trim();

            if (string.IsNullOrEmpty(sing_na2))
            {
                sing_na2 = string.Empty;
            }
            sing_na2 = sing_na2.Trim();

            string sButton = submitbutton.ToUpper();

            if (sButton == "CANCEL")
            {
                temp_state = JsonUtil.SetJsonStringFromObject(mState);
                return RedirectToAction(nameof(SongsList), new { song_state = temp_state });
            }

            string searchType = search_type.Trim().ToUpper();
            Song song = new Song(); // new object
            if (searchType == "SONG_NO")
            {
                // find one song by song_no
                mState.OrderBy = "SongNo";
                song.SongNo = song_no;
            }
            else if (searchType == "SONG_NA")
            {
                // find one song by song_na
                mState.OrderBy = "SongNa";
                song.SongNa = song_na;
            }
            else if (searchType == "VOD_NO")
            {
                // find one song by vod_no
                mState.OrderBy = "VodNo";
                song.VodNo = vod_no;
            }
            else if (searchType == "LANG_SONGNA")
            {
                // find one song by Language + Song.SongNa
                mState.OrderBy = "LangSongNa";   // lang_no + song name
                song.Language = new Language();
                song.Language.LangNo = lang_no;
                song.SongNa = song_na;
            }
            else if (searchType == "SINGER1_NA")
            {
                // find one song by Singer1Na
                mState.OrderBy = "Singer1Na";   // the name of first singer
                song.Singer1 = new Singer();
                song.Singer1.SingNa = sing_na1;
            }
            else if (searchType == "SINGER2_NA")
            {
                // find one song by Singer2Na
                mState.OrderBy = "Singer2Na";   // the name of second singer
                song.Singer2 = new Singer();
                song.Singer2.SingNa = sing_na2;
            }

            List<Song> songsTemp = await _songManager.FindOnePageOfSongsForOneSong(mState, song, 0);
            temp_state = JsonUtil.SetJsonStringFromObject(mState);

            ViewBag.SongState = temp_state;
            return View(nameof(SongsList), songsTemp);
        }

        // Get
        [HttpGet, ActionName("Print")]
        public IActionResult Print()
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            ViewData["Message"] = "Under construction now ..........";
            return View();
        }

        // Get: mothed
        [HttpGet,ActionName("FirstPage")]
        public IActionResult FirstPage(string song_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            StateOfRequest mState;
            if (string.IsNullOrEmpty(song_state))
            {
                mState = new StateOfRequest("SongNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(song_state);
            }
            mState.CurrentPageNo = 1;    // go to first page
            mState.StartTime = DateTime.Now;
            string temp_state = JsonUtil.SetJsonStringFromObject(mState);

            return RedirectToAction(nameof(SongsList), new { song_state = temp_state });
        }

        // Get: mothed
        [HttpGet,ActionName("LastPage")]
        public IActionResult LastPage(string song_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            StateOfRequest mState;
            if (string.IsNullOrEmpty(song_state))
            {
                mState = new StateOfRequest("SongNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(song_state);
            }
            mState.StartTime = DateTime.Now;
            // mState.CurrentPageNo = Int32.MaxValue / mState.PageSize;  // default value for View
            mState.CurrentPageNo = -1;   // present last page
            string temp_state = JsonUtil.SetJsonStringFromObject(mState);

            return RedirectToAction(nameof(SongsList), new { song_state = temp_state });
        }

        // Get: mothed
        [HttpGet,ActionName("PreviousPage")]
        public IActionResult PreviousPage(string song_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            StateOfRequest mState;
            if (string.IsNullOrEmpty(song_state))
            {
                mState = new StateOfRequest("SongNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(song_state);
            }
            mState.StartTime = DateTime.Now;
            mState.CurrentPageNo--;    // go to previous page
            string temp_state = JsonUtil.SetJsonStringFromObject(mState);

            return RedirectToAction(nameof(SongsList), new { song_state = temp_state });
        }

        // Get: mothed
        [HttpGet,ActionName("NextPage")]
        public IActionResult NextPage(string song_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            StateOfRequest mState;
            if (string.IsNullOrEmpty(song_state))
            {
                mState = new StateOfRequest("SongNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(song_state);
            }
            mState.StartTime = DateTime.Now;
            mState.CurrentPageNo++;    // go to next page
            string temp_state = JsonUtil.SetJsonStringFromObject(mState);

            return RedirectToAction(nameof(SongsList), new {song_state = temp_state});
        }

        // GET: Song/Add
        // the view of adding songs to Song table
        public async Task<IActionResult> Add(string song_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));


            Song song = new Song(); // create a new Song object
            List<SelectListItem> languageSelectList = await _languageManager.GetSelectListOfLanguages(new StateOfRequest("LangNa"));
            List<SelectListItem> singerSelectList = await _singerManager.GetSelectListOfSingers(new StateOfRequest("SingNa"));

            ViewBag.LanguageList = languageSelectList;
            ViewBag.SingerList = singerSelectList;
            ViewBag.SongState = song_state; // pass the Json string to View
            return View(song);
        }

        // POST: Song/Add
        // Adds a song to Song table
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(string submitbutton, string song_state, [Bind("Id","SongNo,SongNa,SNumWord,NumFw,NumPw,Chor,NMpeg,MMpeg,VodYn,VodNo,Pathname,InDate,LanguageId,Singer1Id,Singer2Id")] Song song)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            StateOfRequest mState;
            if (string.IsNullOrEmpty(song_state))
            {
                mState = new StateOfRequest("SongNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(song_state);
            }
            mState.StartTime = DateTime.Now;
            string temp_state = JsonUtil.SetJsonStringFromObject(mState);

            int orgId = mState.OrgId;
            string sButton = submitbutton.ToUpper();
            if (sButton == "CANCEL")
            {
                Song newSong = new Song();
                List<Song> songsTemp = await _songManager.FindOnePageOfSongsForOneSong(mState, newSong, orgId);
                temp_state = JsonUtil.SetJsonStringFromObject(mState);

                ViewBag.SongState = temp_state;
                return View(nameof(SongsList), songsTemp);
            }
            if (ModelState.IsValid)
            {
                int result = await _songManager.AddOneSongToTable(song);
                if (result == ErrorCodeModel.Succeeded)
                {
                    // succeeded to add the song
                    // Song newSong = new Song();
                    mState.OrgId = song.Id;
                    mState.OrgNo = song.SongNo;
                    // List<Song> songsTemp = await _songManager.FindOnePageOfSongsForOneSong(mState, newSong, mState.OrgId);
                    // add another song (one more). Go to Get method of Add
                    temp_state = JsonUtil.SetJsonStringFromObject(mState);

                    return RedirectToAction(nameof(Add), new { song_state = temp_state });
                }
                else
                {
                    ViewData["ErrorMessage"] = ErrorCodeModel.GetErrorMessage(result);
                }
            }
            else
            {
                // Model.IsValid = false
                ViewData["ErrorMessage"] = ErrorCodeModel.GetErrorMessage(ErrorCodeModel.ModelBindingFailed);
            }

            ViewBag.SongState = temp_state;
            return View(song);
        }

        // GET: Song/Edit/5
        public async Task<IActionResult> Edit(string song_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            StateOfRequest mState;
            if (string.IsNullOrEmpty(song_state))
            {
                mState = new StateOfRequest("SongNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(song_state);
            }

            int id = mState.OrgId;
            Song song = await _songManager.FindOneSongById(id);

            if (song == null)
            {
                // go to previous view (List view)
                return Redirect(HttpContext.Request.Headers["Referer"]);
            }
            else
            {
                mState.OrgId = song.Id;
                mState.OrgNo = song.SongNo;
                string temp_state = JsonUtil.SetJsonStringFromObject(mState);

                List<SelectListItem> languageSelectList = await _languageManager.GetSelectListOfLanguages(new StateOfRequest("LangNa"));
                List<SelectListItem> singerSelectList = await _singerManager.GetSelectListOfSingers(new StateOfRequest("SingNa"));

                ViewBag.LanguageList = languageSelectList;
                ViewBag.SingerList = singerSelectList;
                ViewBag.SongState = temp_state;
                return View(song);
            }
        }

        // POST: Song/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string submitbutton, string song_state, [Bind("Id","SongNo,SongNa,SNumWord,NumFw,NumPw,Chor,NMpeg,MMpeg,VodYn,VodNo,Pathname,InDate,LanguageId,Singer1Id,Singer2Id")] Song song)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            StateOfRequest mState;
            if (string.IsNullOrEmpty(song_state))
            {
                mState = new StateOfRequest("SongNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(song_state);
            }
            mState.StartTime = DateTime.Now;
            string temp_state = JsonUtil.SetJsonStringFromObject(mState);

            int orgId = mState.OrgId;    // = song.Id
            string sButton = submitbutton.ToUpper();
            if (sButton == "CANCEL")
            {
                temp_state = JsonUtil.SetJsonStringFromObject(mState);
                return RedirectToAction(nameof(SongsList), new { song_state = temp_state });
            }
            if (ModelState.IsValid)
            {
                // start updating table
                int  result = await _songManager.UpdateOneSongById(orgId, song);
                if (result == ErrorCodeModel.Succeeded)
                {
                    // succeeded to update
                    Song newSong = new Song();
                    List<Song> songsTemp = await _songManager.FindOnePageOfSongsForOneSong(mState, newSong, orgId);
                    temp_state = JsonUtil.SetJsonStringFromObject(mState);

                    ViewBag.SongState = temp_state;
                    return View(nameof(SongsList), songsTemp);
                }
                else
                {
                    ViewData["ErrorMessage"] = ErrorCodeModel.GetErrorMessage(result);
                }
            }
            else
            {
                // Model.IsValid = false
                ViewData["ErrorMessage"] = ErrorCodeModel.GetErrorMessage(ErrorCodeModel.ModelBindingFailed);
            }

            List<SelectListItem> languageSelectList = await _languageManager.GetSelectListOfLanguages(new StateOfRequest("LangNa"));
            List<SelectListItem> singerSelectList = await _singerManager.GetSelectListOfSingers(new StateOfRequest("SingNa"));
            ViewBag.LanguageList = languageSelectList;
            ViewBag.SingerList = singerSelectList;
            ViewBag.SongState = temp_state;

            return View(song);
        }

        // GET: Song/Delete/5
        public async Task<IActionResult> Delete(string song_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            StateOfRequest mState;
            if (string.IsNullOrEmpty(song_state))
            {
                mState = new StateOfRequest("SongNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(song_state);
            }
            int id = mState.OrgId;
            Song song = await _songManager.FindOneSongById(id);

            if (song == null)
            {
                // go to previous view (List view)
                return Redirect(HttpContext.Request.Headers["Referer"]);
            }
            else
            {
                mState.OrgId = id;
                mState.OrgNo = song.SongNo;
                string temp_state = JsonUtil.SetJsonStringFromObject(mState);

                List<SelectListItem> languageSelectList = await _languageManager.GetSelectListOfLanguages(new StateOfRequest("LangNa"));
                List<SelectListItem> singerSelectList = await _singerManager.GetSelectListOfSingers(new StateOfRequest("SingNa"));

                ViewBag.LanguageList = languageSelectList;
                ViewBag.SingerList = singerSelectList;
                ViewBag.SongState = temp_state;
                return View(song);
            }
        }

        // POST: Song/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string submitbutton, string song_state, [Bind("Id","SongNo,SongNa,SNumWord,NumFw,NumPw,Chor,NMpeg,MMpeg,VodYn,VodNo,Pathname,InDate,LanguageId,Singer1Id,Singer2Id")] Song song)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            StateOfRequest mState;
            if (string.IsNullOrEmpty(song_state))
            {
                mState = new StateOfRequest("SongNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(song_state);
            }
            mState.StartTime = DateTime.Now;
            string temp_state = JsonUtil.SetJsonStringFromObject(mState);

            int orgId = mState.OrgId;
            string sButton = submitbutton.ToUpper();
            if (sButton == "CANCEL")
            {
                temp_state = JsonUtil.SetJsonStringFromObject(mState);
                return RedirectToAction(nameof(SongsList), new { song_state = temp_state });
            }

            if (ModelState.IsValid)
            {
                // start deleting the song from table
                int result = await _songManager.DeleteOneSongById(orgId);
                if (result == ErrorCodeModel.Succeeded)
                {
                    // succeeded to delete a song
                    List<Song> songsTemp = await _songManager.FindOnePageOfSongsForOneSong(mState, song, 0);
                    temp_state = JsonUtil.SetJsonStringFromObject(mState);

                    ViewBag.SongState = temp_state;
                    return View(nameof(SongsList), songsTemp);
                }
                else
                {
                    ViewData["ErrorMessage"] = ErrorCodeModel.GetErrorMessage(result);
                }
            }
            else
            {
                // Model.IsValid = false
                ViewData["ErrorMessage"] = ErrorCodeModel.GetErrorMessage(ErrorCodeModel.ModelBindingFailed);
            }

            // failed
            List<SelectListItem> languageSelectList = await _languageManager.GetSelectListOfLanguages(new StateOfRequest("LangNa"));
            List<SelectListItem> singerSelectList = await _singerManager.GetSelectListOfSingers(new StateOfRequest("SingNa"));
            ViewBag.LanguageList = languageSelectList;
            ViewBag.SingerList = singerSelectList;
            ViewBag.SongState = temp_state;

            return View(song);
        }

        // GET: Song/5
        public async Task<IActionResult> Details(string song_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            StateOfRequest mState;
            if (string.IsNullOrEmpty(song_state))
            {
                mState = new StateOfRequest("SongNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(song_state);
            }
            Song song = await _songManager.FindOneSongById(mState.OrgId);

            if (song == null)
            {
                // go to previous view (List view)
                return Redirect(HttpContext.Request.Headers["Referer"]);
            }
            else
            {
                mState.OrgId = song.Id;
                mState.OrgNo = song.SongNo;
                string temp_state = JsonUtil.SetJsonStringFromObject(mState);

                List<SelectListItem> languageSelectList = await _languageManager.GetSelectListOfLanguages(new StateOfRequest("LangNa"));
                List<SelectListItem> singerSelectList = await _singerManager.GetSelectListOfSingers(new StateOfRequest("SingNa"));

                ViewBag.LanguageList = languageSelectList;
                ViewBag.SingerList = singerSelectList;
                ViewBag.SongState = temp_state;
                return View(song);
            }
        }

        // POST:
        [HttpPost, ActionName("Details")]
        public async Task<IActionResult> DetailsReturn(string song_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            StateOfRequest mState;
            if (string.IsNullOrEmpty(song_state))
            {
                mState = new StateOfRequest("SongNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(song_state);
            }
            mState.StartTime = DateTime.Now;

            int orgId = mState.OrgId;
            Song song = new Song();
            List<Song> songsTemp = await _songManager.FindOnePageOfSongsForOneSong(mState, song, orgId);
            string temp_state = JsonUtil.SetJsonStringFromObject(mState);

            ViewBag.SongState = temp_state;
            return View(nameof(SongsList), songsTemp);
        }

        // Get:
        [HttpGet,ActionName("ChangeOrder")]
        public async Task<IActionResult> ChangeOrder(string song_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            StateOfRequest mState;
            if (string.IsNullOrEmpty(song_state))
            {
                mState = new StateOfRequest("SongNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(song_state);
            }
            mState.StartTime = DateTime.Now;

            int orgId = 0;
            if (mState.OrgId == 0)
            {
                // no song found or selected in this page
                // then use the first song of this page
                orgId = mState.FirstId;
            }
            else
            {
                orgId = mState.OrgId;
            }

            if (orgId != 0)
            {
                Song song = new Song();
                List<Song> songsTemp = await _songManager.FindOnePageOfSongsForOneSong(mState, song, orgId);
                string temp_state = JsonUtil.SetJsonStringFromObject(mState);

                ViewBag.SongState = temp_state;
                return View(nameof(SongsList), songsTemp);
            }
            else
            {
                // return to the previous page
                return Redirect(HttpContext.Request.Headers["Referer"]);
            }
        }
    }
}
