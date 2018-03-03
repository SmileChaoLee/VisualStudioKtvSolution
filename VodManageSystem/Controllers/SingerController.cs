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

            SingerStateOfRequest singerState = JsonUtil.GetObjectFromJsonString<SingerStateOfRequest>(singer_state);
            string temp_state = JsonUtil.SetJsonStringFromObject(singerState);
            // go to singers management main menu
            return RedirectToAction(nameof(SingersList), new { singer_state = temp_state });    // (action, parameters)
        }

        // Get: get method
        [HttpGet, ActionName("SingersList")]
        public async Task<IActionResult> SingersList(string singer_state)
        {
            SingerStateOfRequest singerState = JsonUtil.GetObjectFromJsonString<SingerStateOfRequest>(singer_state);
            List<Singer> singers = await _singerManager.GetOnePageOfSingersDictionary(singerState);

            ViewBag.SingerState = JsonUtil.SetJsonStringFromObject(singerState);
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

            SingerStateOfRequest singerState = JsonUtil.GetObjectFromJsonString<SingerStateOfRequest>(singer_state);
            singerState.StartTime = DateTime.Now;
            string temp_state = JsonUtil.SetJsonStringFromObject(singerState);

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
                temp_state = JsonUtil.SetJsonStringFromObject(singerState);
                return RedirectToAction(nameof(SingersList), new { singer_state = temp_state });
            }

            string searchType = search_type.Trim().ToUpper();
            Singer singer = new Singer(); // new object
            if (searchType == "SINGER_NO")
            {
                // find one singer by sing_no
                singerState.OrderBy = "SingNo";
                singer.SingNo = sing_no;
            }
            else if (searchType == "SINGER_NA")
            {
                // find one singer by sing_na
                singerState.OrderBy = "SingNa";
                singer.SingNa = sing_na;
            }
            else
            {
                // search_type not defined
                return View();
            }

            List<Singer> singersTemp = await _singerManager.FindOnePageOfSingersForOneSinger(singerState, singer, 0);
            temp_state = JsonUtil.SetJsonStringFromObject(singerState);

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

            SingerStateOfRequest singerState = JsonUtil.GetObjectFromJsonString<SingerStateOfRequest>(singer_state);
            singerState.CurrentPageNo = 1;    // go to first page
            singerState.StartTime = DateTime.Now;

            string temp_state = JsonUtil.SetJsonStringFromObject(singerState);
            return RedirectToAction(nameof(SingersList), new { singer_state = temp_state });
        }

        // Get: mothed
        [HttpGet, ActionName("LastPage")]
        public IActionResult LastPage(string singer_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            SingerStateOfRequest singerState = JsonUtil.GetObjectFromJsonString<SingerStateOfRequest>(singer_state);
            singerState.StartTime = DateTime.Now;
            singerState.CurrentPageNo = Int32.MaxValue / SingerManager.pageSize;

            string temp_state = JsonUtil.SetJsonStringFromObject(singerState);
            return RedirectToAction(nameof(SingersList), new { singer_state = temp_state });
        }

        // Get: mothed
        [HttpGet, ActionName("PreviousPage")]
        public IActionResult PreviousPage(string singer_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            SingerStateOfRequest singerState = JsonUtil.GetObjectFromJsonString<SingerStateOfRequest>(singer_state);
            singerState.StartTime = DateTime.Now;
            singerState.CurrentPageNo--;    // go to previous page

            string temp_state = JsonUtil.SetJsonStringFromObject(singerState);
            return RedirectToAction(nameof(SingersList), new { singer_state = temp_state });
        }

        // Get: mothed
        [HttpGet, ActionName("NextPage")]
        public IActionResult NextPage(string singer_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            SingerStateOfRequest singerState = JsonUtil.GetObjectFromJsonString<SingerStateOfRequest>(singer_state);
            singerState.StartTime = DateTime.Now;
            singerState.CurrentPageNo++;    // go to next page

            string temp_state = JsonUtil.SetJsonStringFromObject(singerState);
            return RedirectToAction(nameof(SingersList), new { singer_state = temp_state });
        }

        // GET: Singer/Add
        // the view of adding singers to Singer table
        public async Task<IActionResult> Add(string singer_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            Singer singer = new Singer(); // create a new Singer object

            List<SelectListItem> singareaSelectList = await _singareaManager.GetSelectListOfSingareas(new SingareaStateOfRequest());

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

            SingerStateOfRequest singerState = JsonUtil.GetObjectFromJsonString<SingerStateOfRequest>(singer_state);
            singerState.StartTime = DateTime.Now;
            string temp_state = JsonUtil.SetJsonStringFromObject(singerState);

            int orgId = singerState.OrgId;
            string sButton = submitbutton.ToUpper();
            if (sButton == "CANCEL")
            {
                Singer newSinger = new Singer();
                List<Singer> singersTemp = await _singerManager.FindOnePageOfSingersForOneSinger(singerState, newSinger, orgId);
                temp_state = JsonUtil.SetJsonStringFromObject(singerState);

                ViewBag.SingerState = temp_state;
                return View(nameof(SingersList), singersTemp);
            }
            if (ModelState.IsValid)
            {
                int result = await _singerManager.AddOneSingerToTable(singer);
                if (result == ErrorCodeModel.Succeeded)
                {
                    // succeeded to add the singer
                    singerState.OrgId = singer.Id;
                    singerState.OrgSingNo = singer.SingNo;

                    temp_state = JsonUtil.SetJsonStringFromObject(singerState);
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

            SingerStateOfRequest singerState = JsonUtil.GetObjectFromJsonString<SingerStateOfRequest>(singer_state);

            int id = singerState.OrgId;
            Singer singer = await _singerManager.FindOneSingerById(id);

            if (singer == null)
            {
                // go to previous view (List view)
                return Redirect(HttpContext.Request.Headers["Referer"]);
            }
            else
            {
                singerState.OrgId = singer.Id;
                singerState.OrgSingNo = singer.SingNo;
                string temp_state = JsonUtil.SetJsonStringFromObject(singerState);

                List<SelectListItem> singareaSelectList = await _singareaManager.GetSelectListOfSingareas(new SingareaStateOfRequest());

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

            SingerStateOfRequest singerState = JsonUtil.GetObjectFromJsonString<SingerStateOfRequest>(singer_state);
            singerState.StartTime = DateTime.Now;
            string temp_state = JsonUtil.SetJsonStringFromObject(singerState);

            int orgId = singerState.OrgId;    // = singer.Id
            string sButton = submitbutton.ToUpper();
            if (sButton == "CANCEL")
            {
                temp_state = JsonUtil.SetJsonStringFromObject(singerState);
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
                    List<Singer> singersTemp = await _singerManager.FindOnePageOfSingersForOneSinger(singerState, newSinger, orgId);
                    temp_state = JsonUtil.SetJsonStringFromObject(singerState);

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

            SingerStateOfRequest singerState = JsonUtil.GetObjectFromJsonString<SingerStateOfRequest>(singer_state);
            int id = singerState.OrgId;
            Singer singer = await _singerManager.FindOneSingerById(id);

            if (singer == null)
            {
                // go to previous view (List view)
                return Redirect(HttpContext.Request.Headers["Referer"]);
            }
            else
            {
                singerState.OrgId = id;
                singerState.OrgSingNo = singer.SingNo;
                string temp_state = JsonUtil.SetJsonStringFromObject(singerState);

                List<SelectListItem> singareaSelectList = await _singareaManager.GetSelectListOfSingareas(new SingareaStateOfRequest());

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

            SingerStateOfRequest singerState = JsonUtil.GetObjectFromJsonString<SingerStateOfRequest>(singer_state);
            singerState.StartTime = DateTime.Now;
            string temp_state = JsonUtil.SetJsonStringFromObject(singerState);

            int orgId = singerState.OrgId;
            string sButton = submitbutton.ToUpper();
            if (sButton == "CANCEL")
            {
                temp_state = JsonUtil.SetJsonStringFromObject(singerState);
                return RedirectToAction(nameof(SingersList), new { singer_state = temp_state });
            }

            if (ModelState.IsValid)
            {
                // start deleting the singer from table
                int result = await _singerManager.DeleteOneSingerById(orgId);
                if (result == ErrorCodeModel.Succeeded)
                {
                    // succeeded to delete a singer
                    List<Singer> singersTemp = await _singerManager.FindOnePageOfSingersForOneSinger(singerState, singer, 0);
                    temp_state = JsonUtil.SetJsonStringFromObject(singerState);

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

            SingerStateOfRequest singerState = JsonUtil.GetObjectFromJsonString<SingerStateOfRequest>(singer_state);
            Singer singer = await _singerManager.FindOneSingerById(singerState.OrgId);

            if (singer == null)
            {
                // go to previous view (List view)
                return Redirect(HttpContext.Request.Headers["Referer"]);
            }
            else
            {
                singerState.OrgId = singer.Id;
                singerState.OrgSingNo = singer.SingNo;
                string temp_state = JsonUtil.SetJsonStringFromObject(singerState);

                List<SelectListItem> singareaSelectList = await _singareaManager.GetSelectListOfSingareas(new SingareaStateOfRequest());

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

            SingerStateOfRequest singerState = JsonUtil.GetObjectFromJsonString<SingerStateOfRequest>(singer_state);
            singerState.StartTime = DateTime.Now;

            int orgId = singerState.OrgId;
            Singer singer = new Singer();
            List<Singer> singersTemp = await _singerManager.FindOnePageOfSingersForOneSinger(singerState, singer, orgId);
            string temp_state = JsonUtil.SetJsonStringFromObject(singerState);

            ViewBag.SingerState = temp_state;
            return View(nameof(SingersList), singersTemp);
        }

        // Get:
        [HttpGet, ActionName("ChangeOrder")]
        public async Task<IActionResult> ChangeOrder(string singer_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            SingerStateOfRequest singerState = JsonUtil.GetObjectFromJsonString<SingerStateOfRequest>(singer_state);
            singerState.StartTime = DateTime.Now;

            int orgId = 0;
            if (singerState.OrgId == 0)
            {
                // no singer found or selected in this page
                // then use the first singer of this page
                orgId = singerState.FirstId;
            }
            else
            {
                orgId = singerState.OrgId;
            }

            if (orgId != 0)
            {
                Singer singer = new Singer();
                List<Singer> singersTemp = await _singerManager.FindOnePageOfSingersForOneSinger(singerState, singer, orgId);
                string temp_state = JsonUtil.SetJsonStringFromObject(singerState);

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
