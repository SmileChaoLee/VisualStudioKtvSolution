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
    public class SongsController : Controller
    {
        private readonly KtvSystemDBContext _context;
        private readonly SongsManager _songsManager;
        private readonly LanguagesManager _languagesManager;
        private readonly SingersManager _singersManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:VodManageSystem.Controllers.SongController"/> class.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="songsManager">Song manager.</param>
        /// <param name="languagesManager">Language manager.</param>
        /// <param name="singersManager">Singer manager.</param>
        public SongsController(KtvSystemDBContext context, SongsManager songsManager, LanguagesManager languagesManager, SingersManager singersManager)
        {
            _context = context;
            _songsManager = songsManager;
            _languagesManager = languagesManager;
            _singersManager = singersManager;
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

            return RedirectToAction(nameof(SongsList), new { song_state = temp_state });    // (action, parameters)
        }

        // get a list of songs
        // Get method.  
        [HttpGet, ActionName("SongsList")]
        public IActionResult SongsList(string song_state)
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

            List<Song> songs = _songsManager.GetOnePageOfSongs(mState);

            ViewBag.SongState = JsonUtil.SetJsonStringFromObject(mState);

            return View(songs);
        }

        // Get
        [HttpGet, ActionName("Find")]
        public IActionResult Find(string song_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            List<SelectListItem> languageSelectList = _languagesManager.GetSelectListOfLanguages(new StateOfRequest("LangNa"));
            List<SelectListItem> singerSelectList = _singersManager.GetSelectListOfSingers(new StateOfRequest("SingNa"));

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
                Language language = await _languagesManager.FindOneLanguageById(languageId);
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

            if (string.IsNullOrEmpty(search_type))
            {
                // search_type not defined
                return View();
            }

            string searchType = search_type.Trim();
            mState.OrderBy = searchType;

            Song song = new Song(); // new object

            song.SongNo = song_no;  // for order by "SongNo"

            song.SongNa = song_na;  // for order by "SongNa"

            song.VodNo = vod_no;    // for order by "VodNo"

            song.Language = new Language(); // for order by "LangNo" + "SongNa"
            song.Language.LangNo = lang_no;
            song.SongNa = song_na;

            song.Singer1 = new Singer();    // for order by "Singer1.SingNa"
            song.Singer1.SingNa = sing_na1;

            song.Singer2 = new Singer();    // for order by "Singer2.SingNa"
            song.Singer2.SingNa = sing_na2;

            List<Song> songsTemp = _songsManager.FindOnePageOfSongsForOneSong(mState, song, -1);
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
        public IActionResult Add(string song_state)
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

            if ( (mState.IsFirstAddRecord ) || (mState.OrgId == 0) )
            {
                // the first id of this page became the selected original id
                // or SateOfRequest.OrgId = 0
                mState.OrgId = mState.FirstId;
            }

            List<SelectListItem> languageSelectList = _languagesManager.GetSelectListOfLanguages(new StateOfRequest("LangNa"));
            List<SelectListItem> singerSelectList = _singersManager.GetSelectListOfSingers(new StateOfRequest("SingNa"));
            ViewBag.LanguageList = languageSelectList;
            ViewBag.SingerList = singerSelectList;

            mState.StartTime = DateTime.Now;
            string temp_state = JsonUtil.SetJsonStringFromObject(mState);
            ViewBag.SongState = temp_state; // pass the Json string to View

            Song song = new Song(); // create a new Song object

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
                List<Song> songsTemp = _songsManager.FindOnePageOfSongsForOneSong(mState, newSong, orgId);
                mState.IsFirstAddRecord = true;
                temp_state = JsonUtil.SetJsonStringFromObject(mState);
                ViewBag.SongState = temp_state;

                return View(nameof(SongsList), songsTemp);
            }
            if (ModelState.IsValid)
            {
                int result = await _songsManager.AddOneSongToTable(song);
                if (result == ErrorCodeModel.Succeeded)
                {
                    // succeeded to add the song
                    // Song newSong = new Song();
                    mState.OrgId = song.Id;
                    mState.OrgNo = song.SongNo;
                    mState.IsFirstAddRecord = false;    // becomes not the first add
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

            List<SelectListItem> languageSelectList = _languagesManager.GetSelectListOfLanguages(new StateOfRequest("LangNa"));
            List<SelectListItem> singerSelectList = _singersManager.GetSelectListOfSingers(new StateOfRequest("SingNa"));
            ViewBag.LanguageList = languageSelectList;
            ViewBag.SingerList = singerSelectList;
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
            Song song = await _songsManager.FindOneSongById(id);

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

                List<SelectListItem> languageSelectList = _languagesManager.GetSelectListOfLanguages(new StateOfRequest("LangNa"));
                List<SelectListItem> singerSelectList = _singersManager.GetSelectListOfSingers(new StateOfRequest("SingNa"));
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
                int  result = await _songsManager.UpdateOneSongById(orgId, song);
                if (result == ErrorCodeModel.Succeeded)
                {
                    // succeeded to update
                    Song newSong = new Song();
                    List<Song> songsTemp = _songsManager.FindOnePageOfSongsForOneSong(mState, newSong, orgId);
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

            List<SelectListItem> languageSelectList = _languagesManager.GetSelectListOfLanguages(new StateOfRequest("LangNa"));
            List<SelectListItem> singerSelectList = _singersManager.GetSelectListOfSingers(new StateOfRequest("SingNa"));
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
            Song song = await _songsManager.FindOneSongById(id);

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

                List<SelectListItem> languageSelectList = _languagesManager.GetSelectListOfLanguages(new StateOfRequest("LangNa"));
                List<SelectListItem> singerSelectList = _singersManager.GetSelectListOfSingers(new StateOfRequest("SingNa"));

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
                int result = await _songsManager.DeleteOneSongById(orgId);
                if (result == ErrorCodeModel.Succeeded)
                {
                    // succeeded to delete a song
                    List<Song> songsTemp = _songsManager.GetOnePageOfSongs(mState);
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
            List<SelectListItem> languageSelectList = _languagesManager.GetSelectListOfLanguages(new StateOfRequest("LangNa"));
            List<SelectListItem> singerSelectList = _singersManager.GetSelectListOfSingers(new StateOfRequest("SingNa"));
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
            Song song = await _songsManager.FindOneSongById(mState.OrgId);

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

                List<SelectListItem> languageSelectList = _languagesManager.GetSelectListOfLanguages(new StateOfRequest("LangNa"));
                List<SelectListItem> singerSelectList = _singersManager.GetSelectListOfSingers(new StateOfRequest("SingNa"));

                ViewBag.LanguageList = languageSelectList;
                ViewBag.SingerList = singerSelectList;
                ViewBag.SongState = temp_state;

                return View(song);
            }
        }

        // POST:
        [HttpPost, ActionName("Details")]
        public IActionResult DetailsReturn(string song_state)
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
            List<Song> songsTemp = _songsManager.GetOnePageOfSongs(mState);
            string temp_state = JsonUtil.SetJsonStringFromObject(mState);

            ViewBag.SongState = temp_state;

            return View(nameof(SongsList), songsTemp);
        }

        // Get:
        [HttpGet,ActionName("ChangeOrder")]
        public IActionResult ChangeOrder(string song_state)
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

            // Added on 2018-11-24
            // start from first page
            mState.CurrentPageNo = 1;
            List<Song> songsTemp = _songsManager.GetOnePageOfSongs(mState);
            string temp_state = JsonUtil.SetJsonStringFromObject(mState);
            ViewBag.SongState = temp_state;
            return View(nameof(SongsList), songsTemp);

            /*
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
                List<Song> songsTemp = _songsManager.FindOnePageOfSongsForOneSong(mState, song, orgId);
                string temp_state = JsonUtil.SetJsonStringFromObject(mState);

                ViewBag.SongState = temp_state;
                return View(nameof(SongsList), songsTemp);
            }

            else
            {
                // return to the previous page
                return Redirect(HttpContext.Request.Headers["Referer"]);
            }
            */
        }
    }
}
