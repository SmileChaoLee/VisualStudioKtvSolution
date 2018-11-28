using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VodManageSystem.Models;
using VodManageSystem.Models.Dao;
using VodManageSystem.Models.DataModels;
using VodManageSystem.Utilities;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VodManageSystem.Controllers
{
    public class SingersController : Controller
    {
        private readonly KtvSystemDBContext _context;
        private readonly SingersManager _singersManager;
        private readonly SingareasManager _singareasManager;

        public SingersController(KtvSystemDBContext context, SingersManager singersManager, SingareasManager singareasManager)
        {
            _context = context;
            _singersManager = singersManager;
            _singareasManager = singareasManager;
        }

        // GET: /<controller>/
        public IActionResult Index(string singer_state)
        {
            // new Index.cshtml
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View();

            StateOfRequest mState;
            if (string.IsNullOrEmpty(singer_state))
            {
                mState = new StateOfRequest("SingNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(singer_state);
            }
            string temp_state = JsonUtil.SetJsonStringFromObject(mState);
            // go to singers management main menu
            return RedirectToAction(nameof(SingersList), new { singer_state = temp_state });    // (action, parameters)
        }

        // Get: get method
        [HttpGet, ActionName("SingersList")]
        public IActionResult SingersList(string singer_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            StateOfRequest mState;
            if (string.IsNullOrEmpty(singer_state))
            {
                mState = new StateOfRequest("SingNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(singer_state);
            }
            List<Singer> singers = _singersManager.GetOnePageOfSingers(mState);

            ViewBag.SingerState = JsonUtil.SetJsonStringFromObject(mState);
            return View(singers);
        }

        // Get
        [HttpGet, ActionName("Find")]
        public IActionResult Find(string singer_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            ViewBag.SingerState = singer_state;
            return View();
        }

        // Post
        [HttpPost, ActionName("Find")]
        public IActionResult Find(string sing_no, string sing_na, string search_type, string submitbutton, string singer_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            StateOfRequest mState;
            if (string.IsNullOrEmpty(singer_state))
            {
                mState = new StateOfRequest("SingNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(singer_state);
            }
            mState.StartTime = DateTime.Now;
            string temp_state = JsonUtil.SetJsonStringFromObject(mState);

            if (string.IsNullOrEmpty(sing_no))
            {
                sing_no = string.Empty;
            }
            sing_no = sing_no.Trim();

            if (string.IsNullOrEmpty(sing_na))
            {
                sing_na = string.Empty;
            }
            sing_na = sing_na.Trim();

            string sButton = submitbutton.ToUpper();

            if (sButton == "CANCEL")
            {
                temp_state = JsonUtil.SetJsonStringFromObject(mState);
                return RedirectToAction(nameof(SingersList), new { singer_state = temp_state });
            }

            if (string.IsNullOrEmpty(search_type))
            {
                // search_type not defined
                return View();
            }

            string searchType = search_type.Trim();
            mState.OrderBy = searchType;

            Singer singer = new Singer(); // new object
            singer.SingNo = sing_no;    // for order by "SingNo"
            singer.SingNa = sing_na;    // for order by "SingNa"

            List<Singer> singersTemp = _singersManager.FindOnePageOfSingersForOneSinger(mState, singer, -1);
            temp_state = JsonUtil.SetJsonStringFromObject(mState);

            ViewBag.SingerState = temp_state;
            return View(nameof(SingersList), singersTemp);
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
        [HttpGet, ActionName("FirstPage")]
        public IActionResult FirstPage(string singer_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            StateOfRequest mState;
            if (string.IsNullOrEmpty(singer_state))
            {
                mState = new StateOfRequest("SingNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(singer_state);
            }
            mState.CurrentPageNo = 1;    // go to first page
            mState.StartTime = DateTime.Now;

            string temp_state = JsonUtil.SetJsonStringFromObject(mState);
            return RedirectToAction(nameof(SingersList), new { singer_state = temp_state });
        }

        // Get: mothed
        [HttpGet, ActionName("LastPage")]
        public IActionResult LastPage(string singer_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            StateOfRequest mState;
            if (string.IsNullOrEmpty(singer_state))
            {
                mState = new StateOfRequest("SingNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(singer_state);
            }
            mState.StartTime = DateTime.Now;
            // mState.CurrentPageNo = Int32.MaxValue / mState.PageSize;  // default  value for View
            mState.CurrentPageNo = -1; // present the last page
            string temp_state = JsonUtil.SetJsonStringFromObject(mState);
            return RedirectToAction(nameof(SingersList), new { singer_state = temp_state });
        }

        // Get: mothed
        [HttpGet, ActionName("PreviousPage")]
        public IActionResult PreviousPage(string singer_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            StateOfRequest mState;
            if (string.IsNullOrEmpty(singer_state))
            {
                mState = new StateOfRequest("SingNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(singer_state);
            }
            mState.StartTime = DateTime.Now;
            mState.CurrentPageNo--;    // go to previous page

            string temp_state = JsonUtil.SetJsonStringFromObject(mState);
            return RedirectToAction(nameof(SingersList), new { singer_state = temp_state });
        }

        // Get: mothed
        [HttpGet, ActionName("NextPage")]
        public IActionResult NextPage(string singer_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            StateOfRequest mState;
            if (string.IsNullOrEmpty(singer_state))
            {
                mState = new StateOfRequest("SingNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(singer_state);
            }
            mState.StartTime = DateTime.Now;
            mState.CurrentPageNo++;    // go to next page

            string temp_state = JsonUtil.SetJsonStringFromObject(mState);
            return RedirectToAction(nameof(SingersList), new { singer_state = temp_state });
        }

        // GET: Singer/Add
        // the view of adding singers to Singer table
        public IActionResult Add(string singer_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            StateOfRequest mState;
            if (string.IsNullOrEmpty(singer_state))
            {
                mState = new StateOfRequest("SingNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(singer_state);
            }

            if ((mState.IsFirstAddRecord) || (mState.OrgId == 0))
            {
                // the first id of this page became the selected original id
                // or SateOfRequest.OrgId = 0
                mState.OrgId = mState.FirstId;
            }

            List<SelectListItem> singareaSelectList = _singareasManager.GetSelectListOfSingareas(new StateOfRequest("AreaNa"));
            ViewBag.SingareaList = singareaSelectList;
            mState.StartTime = DateTime.Now;
            string temp_state = JsonUtil.SetJsonStringFromObject(mState);
            ViewBag.SingerState = temp_state; // pass the Json string to View

            Singer singer = new Singer(); // create a new Singer object

            return View(singer);
        }

        // POST: Singer/Add
        // Adds a singer to Singer table
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(string submitbutton, string singer_state, [Bind("Id", "SingNo, SingNa, NumFw, NumPw, Sex, Chor, Hot, AreaId, PicFile")] Singer singer)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            StateOfRequest mState;
            if (string.IsNullOrEmpty(singer_state))
            {
                mState = new StateOfRequest("SingNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(singer_state);
            }
            mState.StartTime = DateTime.Now;
            string temp_state = JsonUtil.SetJsonStringFromObject(mState);

            int orgId = mState.OrgId;
            string sButton = submitbutton.ToUpper();
            if (sButton == "CANCEL")
            {
                Singer newSinger = new Singer();
                List<Singer> singersTemp = _singersManager.FindOnePageOfSingersForOneSinger(mState, newSinger, orgId);
                mState.IsFirstAddRecord = true;
                temp_state = JsonUtil.SetJsonStringFromObject(mState);
                ViewBag.SingerState = temp_state;

                return View(nameof(SingersList), singersTemp);
            }
            if (ModelState.IsValid)
            {
                int result = await _singersManager.AddOneSingerToTable(singer);
                if (result == ErrorCodeModel.Succeeded)
                {
                    // succeeded to add the singer
                    mState.OrgId = singer.Id;
                    mState.OrgNo = singer.SingNo;
                    mState.IsFirstAddRecord = false;
                    temp_state = JsonUtil.SetJsonStringFromObject(mState);

                    return RedirectToAction(nameof(Add), new { singer_state = temp_state });
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

            List<SelectListItem> singareaSelectList = _singareasManager.GetSelectListOfSingareas(new StateOfRequest("AreaNa"));
            ViewBag.SingareaList = singareaSelectList;
            ViewBag.SingerState = temp_state;

            return View(singer);
        }

        // GET: Singer/Edit/5
        public async Task<IActionResult> Edit(string singer_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            StateOfRequest mState;
            if (string.IsNullOrEmpty(singer_state))
            {
                mState = new StateOfRequest("SingNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(singer_state);
            }
            int id = mState.OrgId;
            Singer singer = await _singersManager.FindOneSingerById(id);

            if (singer == null)
            {
                // go to previous view (List view)
                return Redirect(HttpContext.Request.Headers["Referer"]);
            }
            else
            {
                mState.OrgId = singer.Id;
                mState.OrgNo = singer.SingNo;
                string temp_state = JsonUtil.SetJsonStringFromObject(mState);

                List<SelectListItem> singareaSelectList = _singareasManager.GetSelectListOfSingareas(new StateOfRequest("AreaNa"));
                ViewBag.SingareaList = singareaSelectList;
                ViewBag.SingerState = temp_state;

                return View(singer);
            }
        }

        // POST: Singer/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string submitbutton, string singer_state, [Bind("Id", "SingNo, SingNa, NumFw, NumPw, Sex, Chor, Hot, AreaId, PicFile")] Singer singer)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            StateOfRequest mState;
            if (string.IsNullOrEmpty(singer_state))
            {
                mState = new StateOfRequest("SingNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(singer_state);
            }
            mState.StartTime = DateTime.Now;
            string temp_state = JsonUtil.SetJsonStringFromObject(mState);

            int orgId = mState.OrgId;    // = singer.Id
            string sButton = submitbutton.ToUpper();
            if (sButton == "CANCEL")
            {
                temp_state = JsonUtil.SetJsonStringFromObject(mState);
                return RedirectToAction(nameof(SingersList), new { singer_state = temp_state });
            }
            if (ModelState.IsValid)
            {
                // start updating table
                int result = await _singersManager.UpdateOneSingerById(orgId, singer);
                if (result == ErrorCodeModel.Succeeded)
                {
                    // succeeded to update
                    Singer newSinger = new Singer();
                    List<Singer> singersTemp = _singersManager.FindOnePageOfSingersForOneSinger(mState, newSinger, orgId);
                    temp_state = JsonUtil.SetJsonStringFromObject(mState);

                    ViewBag.SingerState = temp_state;
                    return View(nameof(SingersList), singersTemp);
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

            List<SelectListItem> singareaSelectList = _singareasManager.GetSelectListOfSingareas(new StateOfRequest("AreaNa"));
            ViewBag.SingareaList = singareaSelectList;
            ViewBag.SingerState = temp_state;

            return View(singer);
        }

        // GET: // GET: Singer/Delete/5
        public async Task<IActionResult> Delete(string singer_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            StateOfRequest mState;
            if (string.IsNullOrEmpty(singer_state))
            {
                mState = new StateOfRequest("SingNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(singer_state);
            }
            int id = mState.OrgId;
            Singer singer = await _singersManager.FindOneSingerById(id);

            if (singer == null)
            {
                // go to previous view (List view)
                return Redirect(HttpContext.Request.Headers["Referer"]);
            }
            else
            {
                mState.OrgId = id;
                mState.OrgNo = singer.SingNo;
                string temp_state = JsonUtil.SetJsonStringFromObject(mState);

                List<SelectListItem> singareaSelectList = _singareasManager.GetSelectListOfSingareas(new StateOfRequest("AreaNa"));
                ViewBag.SingareaList = singareaSelectList;
                ViewBag.SingerState = temp_state;

                return View(singer);
            }
        }

        // POST: Singer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string submitbutton, string singer_state, [Bind("Id", "SingNo, SingNa, NumFw, NumPw, Sex, Chor, Hot, AreaId, PicFile")] Singer singer)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            StateOfRequest mState;
            if (string.IsNullOrEmpty(singer_state))
            {
                mState = new StateOfRequest("SingNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(singer_state);
            }
            mState.StartTime = DateTime.Now;
            string temp_state = JsonUtil.SetJsonStringFromObject(mState);

            int orgId = mState.OrgId;
            string sButton = submitbutton.ToUpper();
            if (sButton == "CANCEL")
            {
                temp_state = JsonUtil.SetJsonStringFromObject(mState);
                return RedirectToAction(nameof(SingersList), new { singer_state = temp_state });
            }

            if (ModelState.IsValid)
            {
                // start deleting the singer from table
                int result = await _singersManager.DeleteOneSingerById(orgId);
                if (result == ErrorCodeModel.Succeeded)
                {
                    // succeeded to delete a singer
                    List<Singer> singersTemp = _singersManager.GetOnePageOfSingers(mState);
                    temp_state = JsonUtil.SetJsonStringFromObject(mState);
                    ViewBag.SingerState = temp_state;

                    return View(nameof(SingersList), singersTemp);
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
            List<SelectListItem> singareaSelectList = _singareasManager.GetSelectListOfSingareas(new StateOfRequest("AreaNa"));
            ViewBag.SingareaList = singareaSelectList;
            ViewBag.SingerState = temp_state;

            return View(singer);
        }

        // GET: Singer/5
        public async Task<IActionResult> Details(string singer_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            StateOfRequest mState;
            if (string.IsNullOrEmpty(singer_state))
            {
                mState = new StateOfRequest("SingNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(singer_state);
            }
            Singer singer = await _singersManager.FindOneSingerById(mState.OrgId);

            if (singer == null)
            {
                // go to previous view (List view)
                return Redirect(HttpContext.Request.Headers["Referer"]);
            }
            else
            {
                mState.OrgId = singer.Id;
                mState.OrgNo = singer.SingNo;
                string temp_state = JsonUtil.SetJsonStringFromObject(mState);

                List<SelectListItem> singareaSelectList = _singareasManager.GetSelectListOfSingareas(new StateOfRequest("AreaNa"));
                ViewBag.SingareaList = singareaSelectList;
                ViewBag.SingerState = temp_state;

                return View(singer);
            }
        }

        // POST:
        [HttpPost, ActionName("Details")]
        public IActionResult DetailsReturn(string singer_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            StateOfRequest mState;
            if (string.IsNullOrEmpty(singer_state))
            {
                mState = new StateOfRequest("SingNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(singer_state);
            }
            mState.StartTime = DateTime.Now;

            int orgId = mState.OrgId;
            List<Singer> singersTemp = _singersManager.GetOnePageOfSingers(mState);
            string temp_state = JsonUtil.SetJsonStringFromObject(mState);

            ViewBag.SingerState = temp_state;

            return View(nameof(SingersList), singersTemp);
        }

        // Get:
        [HttpGet, ActionName("ChangeOrder")]
        public IActionResult ChangeOrder(string singer_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            StateOfRequest mState;
            if (string.IsNullOrEmpty(singer_state))
            {
                mState = new StateOfRequest("SingNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(singer_state);
            }
            mState.StartTime = DateTime.Now;

            // Added on 2018-11-24
            // start from first page
            mState.CurrentPageNo = 1;
            List<Singer> singersTemp = _singersManager.GetOnePageOfSingers(mState);
            string temp_state = JsonUtil.SetJsonStringFromObject(mState);
            ViewBag.SingerState = temp_state;

            return View(nameof(SingersList), singersTemp);


            // removed on 2018-11-24
            /*
            int orgId = 0;
            if (mState.OrgId == 0)
            {
                // no singer found or selected in this page
                // then use the first singer of this page
                orgId = mState.FirstId;
            }
            else
            {
                orgId = mState.OrgId;
            }

            if (orgId != 0)
            {
                Singer singer = new Singer();
                List<Singer> singersTemp = _singersManager.FindOnePageOfSingersForOneSinger(mState, singer, orgId);
                string temp_state = JsonUtil.SetJsonStringFromObject(mState);

                ViewBag.SingerState = temp_state;
                return View(nameof(SingersList), singersTemp);
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
