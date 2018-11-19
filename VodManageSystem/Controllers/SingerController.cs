﻿using System;
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
    public class SingerController : Controller
    {
        private readonly KtvSystemDBContext _context;
        private readonly SingerManager _singerManager;
        private readonly SingareaManager _singareaManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:VodManageSystem.Controllers.SingerController"/> class.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="singerManager">Singer manager.</param>
        /// <param name="singareaManager">Singer Area manager.</param>
        public SingerController(KtvSystemDBContext context, SingerManager singerManager, SingareaManager singareaManager)
        {
            _context = context;
            _singerManager = singerManager;
            _singareaManager = singareaManager;
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
        public async Task<IActionResult> SingersList(string singer_state)
        {
            StateOfRequest mState;
            if (string.IsNullOrEmpty(singer_state))
            {
                mState = new StateOfRequest("SingNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(singer_state);
            }
            // List<Singer> singers = await _singerManager.GetOnePageOfSingersDictionary(mState);
            List<Singer> singers = await _singerManager.GetOnePageOfSingers(mState);

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
        public async Task<IActionResult> Find(string sing_no, string sing_na, string search_type, string submitbutton, string singer_state)
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

            string searchType = search_type.Trim().ToUpper();
            Singer singer = new Singer(); // new object
            if (searchType == "SINGER_NO")
            {
                // find one singer by sing_no
                mState.OrderBy = "SingNo";
                singer.SingNo = sing_no;
            }
            else if (searchType == "SINGER_NA")
            {
                // find one singer by sing_na
                mState.OrderBy = "SingNa";
                singer.SingNa = sing_na;
            }
            else
            {
                // search_type not defined
                return View();
            }

            List<Singer> singersTemp = await _singerManager.FindOnePageOfSingersForOneSinger(mState, singer, 0);
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
        public async Task<IActionResult> Add(string singer_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            Singer singer = new Singer(); // create a new Singer object

            List<SelectListItem> singareaSelectList = await _singareaManager.GetSelectListOfSingareas(new StateOfRequest("AreaNa"));

            ViewBag.SingareaList = singareaSelectList;
            ViewBag.SingerState = singer_state; // pass the Json string to View
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
                List<Singer> singersTemp = await _singerManager.FindOnePageOfSingersForOneSinger(mState, newSinger, orgId);
                temp_state = JsonUtil.SetJsonStringFromObject(mState);

                ViewBag.SingerState = temp_state;
                return View(nameof(SingersList), singersTemp);
            }
            if (ModelState.IsValid)
            {
                int result = await _singerManager.AddOneSingerToTable(singer);
                if (result == ErrorCodeModel.Succeeded)
                {
                    // succeeded to add the singer
                    mState.OrgId = singer.Id;
                    mState.OrgNo = singer.SingNo;

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
            Singer singer = await _singerManager.FindOneSingerById(id);

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

                List<SelectListItem> singareaSelectList = await _singareaManager.GetSelectListOfSingareas(new StateOfRequest("AreaNo"));

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
                Console.WriteLine("Edit --> Post --> Id = " + singer.Id);
                Console.WriteLine("Edit --> Post --> AreaId = " + singer.AreaId);

                int result = await _singerManager.UpdateOneSingerById(orgId, singer);
                if (result == ErrorCodeModel.Succeeded)
                {
                    // succeeded to update
                    Singer newSinger = new Singer();
                    List<Singer> singersTemp = await _singerManager.FindOnePageOfSingersForOneSinger(mState, newSinger, orgId);
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
            Singer singer = await _singerManager.FindOneSingerById(id);

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

                List<SelectListItem> singareaSelectList = await _singareaManager.GetSelectListOfSingareas(new StateOfRequest("AreaNa"));

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
                int result = await _singerManager.DeleteOneSingerById(orgId);
                if (result == ErrorCodeModel.Succeeded)
                {
                    // succeeded to delete a singer
                    List<Singer> singersTemp = await _singerManager.FindOnePageOfSingersForOneSinger(mState, singer, 0);
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
            Singer singer = await _singerManager.FindOneSingerById(mState.OrgId);

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

                List<SelectListItem> singareaSelectList = await _singareaManager.GetSelectListOfSingareas(new StateOfRequest("AreaNo"));

                ViewBag.SingareaList = singareaSelectList;
                ViewBag.SingerState = temp_state;
                return View(singer);
            }
        }

        // POST:
        [HttpPost, ActionName("Details")]
        public async Task<IActionResult> DetailsReturn(string singer_state)
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
            Singer singer = new Singer();
            List<Singer> singersTemp = await _singerManager.FindOnePageOfSingersForOneSinger(mState, singer, orgId);
            string temp_state = JsonUtil.SetJsonStringFromObject(mState);

            ViewBag.SingerState = temp_state;
            return View(nameof(SingersList), singersTemp);
        }

        // Get:
        [HttpGet, ActionName("ChangeOrder")]
        public async Task<IActionResult> ChangeOrder(string singer_state)
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
                List<Singer> singersTemp = await _singerManager.FindOnePageOfSingersForOneSinger(mState, singer, orgId);
                string temp_state = JsonUtil.SetJsonStringFromObject(mState);

                ViewBag.SingerState = temp_state;
                return View(nameof(SingersList), singersTemp);
            }
            else
            {
                // return to the previous page
                return Redirect(HttpContext.Request.Headers["Referer"]);
            }
        }
    }
}
