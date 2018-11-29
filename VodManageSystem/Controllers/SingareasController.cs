using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VodManageSystem.Models;
using VodManageSystem.Models.Dao;
using VodManageSystem.Models.DataModels;
using VodManageSystem.Utilities;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VodManageSystem.Controllers
{
    public class SingareasController : Controller
    {
        private readonly KtvSystemDBContext _context;
        private readonly SingareasManager _singareasManager;

        public SingareasController(KtvSystemDBContext context, SingareasManager singareasManager)
        {
            _context = context;
            _singareasManager = singareasManager;
        }

        // GET: /<controller>/
        // [Route("/[controller]/[action]")]
        public IActionResult Index(string singarea_state)
        {
            // new Index.cshtml
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View();

            StateOfRequest mState;
            if (string.IsNullOrEmpty(singarea_state))
            {
                mState = new StateOfRequest("AreaNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(singarea_state);
            }
            string temp_state = JsonUtil.SetJsonStringFromObject(mState);
            // go to singareas management main menu
            return RedirectToAction(nameof(SingareasList), new { singarea_state = temp_state });    // (action, parameters)
        }

        // Get: get method
        // [Route("/[controller]/[action]")]
        [HttpGet, ActionName("SingareasList")]
        public IActionResult SingareasList(string singarea_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            StateOfRequest mState;
            if (string.IsNullOrEmpty(singarea_state))
            {
                mState = new StateOfRequest("AreaNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(singarea_state);
            }

            List<Singarea> singareas = _singareasManager.GetOnePageOfSingareas(mState);
            ViewBag.SingareaState = JsonUtil.SetJsonStringFromObject(mState);

            return View(singareas);
        }

        // Get
        [HttpGet, ActionName("Find")]
        public IActionResult Find(string singarea_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            ViewBag.SingareaState = singarea_state;
            return View();
        }

        // Post
        [HttpPost, ActionName("Find")]
        public IActionResult Find(string area_no, string area_na, string search_type, string submitbutton, string singarea_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            StateOfRequest mState;
            if (string.IsNullOrEmpty(singarea_state))
            {
                mState = new StateOfRequest("AreaNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(singarea_state);
            }
            mState.StartTime = DateTime.Now;
            string temp_state = JsonUtil.SetJsonStringFromObject(mState);

            if (string.IsNullOrEmpty(area_no))
            {
                area_no = string.Empty;
            }
            area_no = area_no.Trim();

            if (string.IsNullOrEmpty(area_na))
            {
                area_na = string.Empty;
            }
            area_na = area_na.Trim();

            string sButton = submitbutton.ToUpper();

            if (sButton == "CANCEL")
            {
                temp_state = JsonUtil.SetJsonStringFromObject(mState);
                return RedirectToAction(nameof(SingareasList), new { singarea_state = temp_state });
            }
            if (string.IsNullOrEmpty(search_type))
            {
                // search_type not defined
                return View();
            }

            string searchType = search_type.Trim();
            mState.OrderBy = searchType;
            Singarea singarea = new Singarea(); // new object
            singarea.AreaNo = area_no;  // for order by "AreaNo"
            singarea.AreaNa = area_na;  // for order by "AreaNa"

            List<Singarea> singareasTemp = _singareasManager.FindOnePageOfSingareasForOneSingarea(mState, singarea, -1);
            temp_state = JsonUtil.SetJsonStringFromObject(mState);
            ViewBag.SingareaState = temp_state;

            return View(nameof(SingareasList), singareasTemp);
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
        public IActionResult FirstPage(string singarea_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            StateOfRequest mState;
            if (string.IsNullOrEmpty(singarea_state))
            {
                mState = new StateOfRequest("AreaNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(singarea_state);
            }
            mState.CurrentPageNo = 1;    // go to first page
            mState.StartTime = DateTime.Now;

            string temp_state = JsonUtil.SetJsonStringFromObject(mState);
            return RedirectToAction(nameof(SingareasList), new { singarea_state = temp_state });
        }

        // Get: mothed
        [HttpGet, ActionName("LastPage")]
        public IActionResult LastPage(string singarea_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            StateOfRequest mState;
            if (string.IsNullOrEmpty(singarea_state))
            {
                mState = new StateOfRequest("AreaNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(singarea_state);
            }
            mState.StartTime = DateTime.Now;
            // mState.CurrentPageNo = Int32.MaxValue / mState.PageSize;  // default value for View
            mState.CurrentPageNo = -1;   // present the last page
            string temp_state = JsonUtil.SetJsonStringFromObject(mState);

            return RedirectToAction(nameof(SingareasList), new { singarea_state = temp_state });
        }

        // Get: mothed
        [HttpGet, ActionName("PreviousPage")]
        public IActionResult PreviousPage(string singarea_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            StateOfRequest mState;
            if (string.IsNullOrEmpty(singarea_state))
            {
                mState = new StateOfRequest("AreaNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(singarea_state);
            }
            mState.StartTime = DateTime.Now;
            mState.CurrentPageNo--;    // go to previous page
            string temp_state = JsonUtil.SetJsonStringFromObject(mState);

            return RedirectToAction(nameof(SingareasList), new { singarea_state = temp_state });
        }

        // Get: mothed
        [HttpGet, ActionName("NextPage")]
        public IActionResult NextPage(string singarea_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            StateOfRequest mState;
            if (string.IsNullOrEmpty(singarea_state))
            {
                mState = new StateOfRequest("AreaNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(singarea_state);
            }
            mState.StartTime = DateTime.Now;
            mState.CurrentPageNo++;    // go to next page
            string temp_state = JsonUtil.SetJsonStringFromObject(mState);

            return RedirectToAction(nameof(SingareasList), new { singarea_state = temp_state });
        }

        // GET: Singarea/Add
        // the view of adding singareas to Singarea table
        public IActionResult Add(string singarea_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            StateOfRequest mState;
            if (string.IsNullOrEmpty(singarea_state))
            {
                mState = new StateOfRequest("AreaNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(singarea_state);
            }

            if ((mState.IsFirstAddRecord) || (mState.OrgId == 0))
            {
                // the first id of this page became the selected original id
                // or SateOfRequest.OrgId = 0
                mState.OrgId = mState.FirstId;
            }

            mState.StartTime = DateTime.Now;
            string temp_state = JsonUtil.SetJsonStringFromObject(mState);
            ViewBag.SingareaState = temp_state; // pass the Json string to View

            Singarea singarea = new Singarea(); // create a new Singarea object

            return View(singarea);
        }

        // POST: Singarea/Add
        // Adds a singarea to Singarea table
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(string submitbutton, string singarea_state, [Bind("Id", "AreaNo, AreaNa,AreaEn")] Singarea singarea)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            StateOfRequest mState;
            if (string.IsNullOrEmpty(singarea_state))
            {
                mState = new StateOfRequest("AreaNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(singarea_state);
            }
            mState.StartTime = DateTime.Now;
            string temp_state = JsonUtil.SetJsonStringFromObject(mState);

            int orgId = mState.OrgId;
            string sButton = submitbutton.ToUpper();
            if (sButton == "CANCEL")
            {
                Singarea newSingarea = new Singarea();
                List<Singarea> singareasTemp = _singareasManager.FindOnePageOfSingareasForOneSingarea(mState, newSingarea, orgId);
                mState.IsFirstAddRecord = true;
                temp_state = JsonUtil.SetJsonStringFromObject(mState);
                ViewBag.SingareaState = temp_state;

                return View(nameof(SingareasList), singareasTemp);
            }
            if (ModelState.IsValid)
            {
                int result = await _singareasManager.AddOneSingareaToTable(singarea);
                if (result == ErrorCodeModel.Succeeded)
                {
                    // succeeded to add the singarea
                    mState.OrgId = singarea.Id;
                    mState.OrgNo = singarea.AreaNo;
                    mState.IsFirstAddRecord = false;
                    temp_state = JsonUtil.SetJsonStringFromObject(mState);

                    return RedirectToAction(nameof(Add), new { singarea_state = temp_state });
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

            ViewBag.SingareaState = temp_state;
            return View(singarea);
        }

        // GET: Singarea/Edit/5
        public async Task<IActionResult> Edit(string singarea_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            StateOfRequest mState;
            if (string.IsNullOrEmpty(singarea_state))
            {
                mState = new StateOfRequest("AreaNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(singarea_state);
            }
            int id = mState.OrgId;
            Singarea singarea = await _singareasManager.FindOneSingareaById(id);

            if (singarea == null)
            {
                // go to previous view (List view)
                return Redirect(HttpContext.Request.Headers["Referer"]);
            }
            else
            {
                mState.OrgId = singarea.Id;
                mState.OrgNo = singarea.AreaNo;
                string temp_state = JsonUtil.SetJsonStringFromObject(mState);

                ViewBag.SingareaState = temp_state;
                return View(singarea);
            }
        }

        // POST: Singarea/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string submitbutton, string singarea_state, [Bind("Id", "AreaNo, AreaNa, AreaEn")] Singarea singarea)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            StateOfRequest mState;
            if (string.IsNullOrEmpty(singarea_state))
            {
                mState = new StateOfRequest("AreaNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(singarea_state);
            }
            mState.StartTime = DateTime.Now;
            string temp_state = JsonUtil.SetJsonStringFromObject(mState);

            int orgId = mState.OrgId;    // = Singarea.Id
            string sButton = submitbutton.ToUpper();
            if (sButton == "CANCEL")
            {
                temp_state = JsonUtil.SetJsonStringFromObject(mState);
                return RedirectToAction(nameof(SingareasList), new { singarea_state = temp_state });
            }
            if (ModelState.IsValid)
            {
                // start updating table
                int result = await _singareasManager.UpdateOneSingareaById(orgId, singarea);
                if (result == ErrorCodeModel.Succeeded)
                {
                    // succeeded to update
                    Singarea newSingarea = new Singarea();
                    List<Singarea> singareasTemp = _singareasManager.FindOnePageOfSingareasForOneSingarea(mState, newSingarea, orgId);
                    temp_state = JsonUtil.SetJsonStringFromObject(mState);

                    ViewBag.SingareaState = temp_state;
                    return View(nameof(SingareasList), singareasTemp);
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

            ViewBag.SingareaState = temp_state;
            return View(singarea);
        }

        // GET: // GET: Singarea/Delete/5
        public async Task<IActionResult> Delete(string singarea_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            StateOfRequest mState;
            if (string.IsNullOrEmpty(singarea_state))
            {
                mState = new StateOfRequest("AreaNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(singarea_state);
            }
            int id = mState.OrgId;
            Singarea singarea = await _singareasManager.FindOneSingareaById(id);

            if (singarea == null)
            {
                // go to previous view (List view)
                return Redirect(HttpContext.Request.Headers["Referer"]);
            }
            else
            {
                mState.OrgId = id;
                mState.OrgNo = singarea.AreaNo;
                string temp_state = JsonUtil.SetJsonStringFromObject(mState);
                ViewBag.SingareaState = temp_state;

                return View(singarea);
            }
        }

        // POST: Singarea/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string submitbutton, string singarea_state, [Bind("Id","AreaNo, AreaNa, AreaEn")] Singarea singarea)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            StateOfRequest mState;
            if (string.IsNullOrEmpty(singarea_state))
            {
                mState = new StateOfRequest("AreaNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(singarea_state);
            }
            mState.StartTime = DateTime.Now;
            string temp_state = JsonUtil.SetJsonStringFromObject(mState);

            int orgId = mState.OrgId;
            string sButton = submitbutton.ToUpper();
            if (sButton == "CANCEL")
            {
                temp_state = JsonUtil.SetJsonStringFromObject(mState);
                return RedirectToAction(nameof(SingareasList), new { singarea_state = temp_state });
            }

            if (ModelState.IsValid)
            {
                // start deleting the singarea from table
                int result = await _singareasManager.DeleteOneSingareaById(orgId);
                if (result == ErrorCodeModel.Succeeded)
                {
                    // succeeded to delete a singarea
                    List<Singarea> singareasTemp = _singareasManager.GetOnePageOfSingareas(mState);
                    temp_state = JsonUtil.SetJsonStringFromObject(mState);
                    ViewBag.SingareaState = temp_state;

                    return View(nameof(SingareasList), singareasTemp);
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
            ViewBag.SingareaState = temp_state;
            return View(singarea);
        }

        // GET: Singarea/5
        public async Task<IActionResult> Details(string singarea_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            StateOfRequest mState;
            if (string.IsNullOrEmpty(singarea_state))
            {
                mState = new StateOfRequest("AreaNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(singarea_state);
            }
            Singarea singarea = await _singareasManager.FindOneSingareaById(mState.OrgId);

            if (singarea == null)
            {
                // go to previous view (List view)
                return Redirect(HttpContext.Request.Headers["Referer"]);
            }
            else
            {
                mState.OrgId = singarea.Id;
                mState.OrgNo = singarea.AreaNo;
                string temp_state = JsonUtil.SetJsonStringFromObject(mState);

                ViewBag.SingareaState = temp_state;
                return View(singarea);
            }
        }

        // POST:
        [HttpPost, ActionName("Details")]
        public IActionResult DetailsReturn(string singarea_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            StateOfRequest mState;
            if (string.IsNullOrEmpty(singarea_state))
            {
                mState = new StateOfRequest("AreaNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(singarea_state);
            }
            mState.StartTime = DateTime.Now;

            int orgId = mState.OrgId;
            List<Singarea> singareasTemp = _singareasManager.GetOnePageOfSingareas(mState);
            string temp_state = JsonUtil.SetJsonStringFromObject(mState);

            ViewBag.SingareaState = temp_state;

            return View(nameof(SingareasList), singareasTemp);
        }

        // Get:
        [HttpGet, ActionName("ChangeOrder")]
        public IActionResult ChangeOrder(string singarea_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            StateOfRequest mState;
            if (string.IsNullOrEmpty(singarea_state))
            {
                mState = new StateOfRequest("AreaNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(singarea_state);
            }
            mState.StartTime = DateTime.Now;

            // Added on 2018-11-24
            // start from first page
            mState.CurrentPageNo = 1;
            List<Singarea> singareasTemp = _singareasManager.GetOnePageOfSingareas(mState);
            string temp_state = JsonUtil.SetJsonStringFromObject(mState);
            ViewBag.SingareaState = temp_state;

            return View(nameof(SingareasList), singareasTemp);

            /*s
            int orgId = 0;
            if (mState.OrgId == 0)
            {
                // no singarea found or selected in this page
                // then use the first singarea of this page
                orgId = mState.FirstId;
            }
            else
            {
                orgId = mState.OrgId;
            }

            if (orgId != 0)
            {
                Singarea singarea = new Singarea();
                List<Singarea> singareasTemp = _singareasManager.FindOnePageOfSingareasForOneSingarea(mState, singarea, orgId);
                string temp_state = JsonUtil.SetJsonStringFromObject(mState);

                ViewBag.SingareaState = temp_state;
                return View(nameof(SingareasList), singareasTemp);
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
