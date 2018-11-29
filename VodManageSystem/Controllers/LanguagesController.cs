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
    public class LanguagesController : Controller
    {
        private readonly KtvSystemDBContext _context;
        private readonly LanguagesManager _languagesManager;

        public LanguagesController(KtvSystemDBContext context, LanguagesManager languagesManager)
        {
            _context = context;
            _languagesManager = languagesManager;
        }

        // GET: /<controller>/
        public IActionResult Index(string language_state)
        {
            // new Index.cshtml
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View();

            StateOfRequest mState;
            if (string.IsNullOrEmpty(language_state))
            {
                mState = new StateOfRequest("LangNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(language_state);
            }
            string temp_state = JsonUtil.SetJsonStringFromObject(mState);
            // go to languages management main menu
            return RedirectToAction(nameof(LanguagesList), new { language_state = temp_state });    // (action, parameters)
        }

        // Get: get method
        [HttpGet, ActionName("LanguagesList")]
        public IActionResult LanguagesList(string language_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            StateOfRequest mState;
            if (string.IsNullOrEmpty(language_state))
            {
                mState = new StateOfRequest("LangNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(language_state);
            }
            List<Language> languages = _languagesManager.GetOnePageOfLanguages(mState);

            ViewBag.LanguageState = JsonUtil.SetJsonStringFromObject(mState);
            return View(languages);
        }

        // Get
        [HttpGet, ActionName("Find")]
        public IActionResult Find(string language_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            ViewBag.LanguageState = language_state;
            return View();
        }

        // Post
        [HttpPost, ActionName("Find")]
        public IActionResult Find(string lang_no, string lang_na, string search_type, string submitbutton, string language_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            StateOfRequest mState;
            if (string.IsNullOrEmpty(language_state))
            {
                mState = new StateOfRequest("LangNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(language_state);
            }
            mState.StartTime = DateTime.Now;
            string temp_state = JsonUtil.SetJsonStringFromObject(mState);

            if (string.IsNullOrEmpty(lang_no))
            {
                lang_no = string.Empty;
            }
            lang_no = lang_no.Trim();

            if (string.IsNullOrEmpty(lang_na))
            {
                lang_na = string.Empty;
            }
            lang_na = lang_na.Trim();

            string sButton = submitbutton.ToUpper();

            if (sButton == "CANCEL")
            {
                temp_state = JsonUtil.SetJsonStringFromObject(mState);
                return RedirectToAction(nameof(LanguagesList), new { language_state = temp_state });
            }

            if (string.IsNullOrEmpty(search_type))
            {
                // search_type not defined
                return View();
            }

            string searchType = search_type.Trim();
            mState.OrderBy = searchType;
            Language language = new Language(); // new object
            language.LangNo = lang_no;  // for order by "LangNo"
            language.LangNa = lang_na;  // for order by "LangNa"

            List<Language> languagesTemp = _languagesManager.FindOnePageOfLanguagesForOneLanguage(mState, language, -1);
            temp_state = JsonUtil.SetJsonStringFromObject(mState);
            ViewBag.LanguageState = temp_state;

            return View(nameof(LanguagesList), languagesTemp);
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
        public IActionResult FirstPage(string language_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            StateOfRequest mState;
            if (string.IsNullOrEmpty(language_state))
            {
                mState = new StateOfRequest("LangNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(language_state);
            }
            mState.CurrentPageNo = 1;    // go to first page
            mState.StartTime = DateTime.Now;

            string temp_state = JsonUtil.SetJsonStringFromObject(mState);
            return RedirectToAction(nameof(LanguagesList), new { language_state = temp_state });
        }

        // Get: mothed
        [HttpGet, ActionName("LastPage")]
        public IActionResult LastPage(string language_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            StateOfRequest mState;
            if (string.IsNullOrEmpty(language_state))
            {
                mState = new StateOfRequest("LangNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(language_state);
            }
            mState.StartTime = DateTime.Now;
            // mState.CurrentPageNo = Int32.MaxValue / mState.PageSize;  // default value for View
            mState.CurrentPageNo = -1;   // present last page
            string temp_state = JsonUtil.SetJsonStringFromObject(mState);

            return RedirectToAction(nameof(LanguagesList), new { language_state = temp_state });
        }

        // Get: mothed
        [HttpGet, ActionName("PreviousPage")]
        public IActionResult PreviousPage(string language_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            StateOfRequest mState;
            if (string.IsNullOrEmpty(language_state))
            {
                mState = new StateOfRequest("LangNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(language_state);
            }
            mState.StartTime = DateTime.Now;
            mState.CurrentPageNo--;    // go to previous page
            string temp_state = JsonUtil.SetJsonStringFromObject(mState);

            return RedirectToAction(nameof(LanguagesList), new { language_state = temp_state });
        }

        // Get: mothed
        [HttpGet, ActionName("NextPage")]
        public IActionResult NextPage(string language_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            StateOfRequest mState;
            if (string.IsNullOrEmpty(language_state))
            {
                mState = new StateOfRequest("LangNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(language_state);
            }
            mState.StartTime = DateTime.Now;
            mState.CurrentPageNo++;    // go to next page
            string temp_state = JsonUtil.SetJsonStringFromObject(mState);

            return RedirectToAction(nameof(LanguagesList), new { language_state = temp_state });
        }

        // GET: Language/Add
        // the view of adding languages to Language table
        public IActionResult Add(string language_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            StateOfRequest mState;
            if (string.IsNullOrEmpty(language_state))
            {
                mState = new StateOfRequest("LangNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(language_state);
            }

            if ((mState.IsFirstAddRecord) || (mState.OrgId == 0))
            {
                // the first id of this page became the selected original id
                // or SateOfRequest.OrgId = 0
                mState.OrgId = mState.FirstId;
            }

            mState.StartTime = DateTime.Now;
            string temp_state = JsonUtil.SetJsonStringFromObject(mState);
            ViewBag.LanguageState = temp_state; // pass the Json string to View

            Language language = new Language(); // create a new Language object

            return View(language);
        }

        // POST: Language/Add
        // Adds a language to Language table
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(string submitbutton, string language_state, [Bind("Id", "LangNo, LangNa,LangEn")] Language language)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            StateOfRequest mState;
            if (string.IsNullOrEmpty(language_state))
            {
                mState = new StateOfRequest("LangNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(language_state);
            }
            mState.StartTime = DateTime.Now;
            string temp_state = JsonUtil.SetJsonStringFromObject(mState);

            int orgId = mState.OrgId;
            string sButton = submitbutton.ToUpper();
            if (sButton == "CANCEL")
            {
                Language newLanguage = new Language();
                List<Language> languagesTemp = _languagesManager.FindOnePageOfLanguagesForOneLanguage(mState, newLanguage, orgId);
                mState.IsFirstAddRecord = true;
                temp_state = JsonUtil.SetJsonStringFromObject(mState);
                ViewBag.LanguageState = temp_state;

                return View(nameof(LanguagesList), languagesTemp);
            }
            if (ModelState.IsValid)
            {
                int result = await _languagesManager.AddOneLanguageToTable(language);
                if (result == ErrorCodeModel.Succeeded)
                {
                    // succeeded to add the language
                    mState.OrgId = language.Id;
                    mState.OrgNo = language.LangNo;
                    mState.IsFirstAddRecord = false;
                    temp_state = JsonUtil.SetJsonStringFromObject(mState);

                    return RedirectToAction(nameof(Add), new { language_state = temp_state });
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

            ViewBag.LanguageState = temp_state;
            return View(language);
        }

        // GET: Language/Edit/5
        public async Task<IActionResult> Edit(string language_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            StateOfRequest mState;
            if (string.IsNullOrEmpty(language_state))
            {
                mState = new StateOfRequest("LangNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(language_state);
            }

            int id = mState.OrgId;
            Language language = await _languagesManager.FindOneLanguageById(id);

            if (language == null)
            {
                // go to previous view (List view)
                return Redirect(HttpContext.Request.Headers["Referer"]);
            }
            else
            {
                mState.OrgId = language.Id;
                mState.OrgNo = language.LangNo;
                string temp_state = JsonUtil.SetJsonStringFromObject(mState);

                ViewBag.LanguageState = temp_state;
                return View(language);
            }
        }

        // POST: Language/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string submitbutton, string language_state, [Bind("Id", "LangNo, LangNa, LangEn")] Language language)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            StateOfRequest mState;
            if (string.IsNullOrEmpty(language_state))
            {
                mState = new StateOfRequest("LangNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(language_state);
            }
            mState.StartTime = DateTime.Now;
            string temp_state = JsonUtil.SetJsonStringFromObject(mState);

            int orgId = mState.OrgId;    // = language.Id
            string sButton = submitbutton.ToUpper();
            if (sButton == "CANCEL")
            {
                temp_state = JsonUtil.SetJsonStringFromObject(mState);
                return RedirectToAction(nameof(LanguagesList), new { language_state = temp_state });
            }
            if (ModelState.IsValid)
            {
                // start updating table
                int result = await _languagesManager.UpdateOneLanguageById(orgId, language);
                if (result == ErrorCodeModel.Succeeded)
                {
                    // succeeded to update
                    Language newLanguage = new Language();
                    List<Language> languagesTemp = _languagesManager.FindOnePageOfLanguagesForOneLanguage(mState, newLanguage, orgId);
                    temp_state = JsonUtil.SetJsonStringFromObject(mState);

                    ViewBag.LanguageState = temp_state;
                    return View(nameof(LanguagesList), languagesTemp);
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

            ViewBag.LanguageState = temp_state;
            return View(language);
        }

        // GET: // GET: Language/Delete/5
        public async Task<IActionResult> Delete(string language_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            StateOfRequest mState;
            if (string.IsNullOrEmpty(language_state))
            {
                mState = new StateOfRequest("LangNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(language_state);
            }
            int id = mState.OrgId;
            Language language = await _languagesManager.FindOneLanguageById(id);

            if (language == null)
            {
                // go to previous view (List view)
                return Redirect(HttpContext.Request.Headers["Referer"]);
            }
            else
            {
                mState.OrgId = id;
                mState.OrgNo = language.LangNo;
                string temp_state = JsonUtil.SetJsonStringFromObject(mState);

                ViewBag.LanguageState = temp_state;
                return View(language);
            }
        }

        // POST: Language/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string submitbutton, string language_state, [Bind("Id","LangNo, LangNa, LangEn")] Language language)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            StateOfRequest mState;
            if (string.IsNullOrEmpty(language_state))
            {
                mState = new StateOfRequest("LangNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(language_state);
            }
            mState.StartTime = DateTime.Now;
            string temp_state = JsonUtil.SetJsonStringFromObject(mState);

            int orgId = mState.OrgId;
            string sButton = submitbutton.ToUpper();
            if (sButton == "CANCEL")
            {
                temp_state = JsonUtil.SetJsonStringFromObject(mState);
                return RedirectToAction(nameof(LanguagesList), new { language_state = temp_state });
            }

            if (ModelState.IsValid)
            {
                // start deleting the language from table
                int result = await _languagesManager.DeleteOneLanguageById(orgId);
                if (result == ErrorCodeModel.Succeeded)
                {
                    // succeeded to delete a language
                    List<Language> languagesTemp = _languagesManager.GetOnePageOfLanguages(mState);
                    temp_state = JsonUtil.SetJsonStringFromObject(mState);
                    ViewBag.LanguageState = temp_state;

                    return View(nameof(LanguagesList), languagesTemp);
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
            ViewBag.LanguageState = temp_state;
            return View(language);
        }

        // GET: Language/5
        public async Task<IActionResult> Details(string language_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            StateOfRequest mState;
            if (string.IsNullOrEmpty(language_state))
            {
                mState = new StateOfRequest("LangNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(language_state);
            }
            Language language = await _languagesManager.FindOneLanguageById(mState.OrgId);

            if (language == null)
            {
                // go to previous view (List view)
                return Redirect(HttpContext.Request.Headers["Referer"]);
            }
            else
            {
                mState.OrgId = language.Id;
                mState.OrgNo = language.LangNo;
                string temp_state = JsonUtil.SetJsonStringFromObject(mState);
                ViewBag.LanguageState = temp_state;

                return View(language);
            }
        }

        // POST:
        [HttpPost, ActionName("Details")]
        public IActionResult DetailsReturn(string language_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            StateOfRequest mState;
            if (string.IsNullOrEmpty(language_state))
            {
                mState = new StateOfRequest("LangNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(language_state);
            }
            mState.StartTime = DateTime.Now;

            int orgId = mState.OrgId;
            List<Language> languagesTemp = _languagesManager.GetOnePageOfLanguages(mState);
            string temp_state = JsonUtil.SetJsonStringFromObject(mState);
            ViewBag.LanguageState = temp_state;

            return View(nameof(LanguagesList), languagesTemp);
        }

        // Get:
        [HttpGet, ActionName("ChangeOrder")]
        public IActionResult ChangeOrder(string language_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            StateOfRequest mState;
            if (string.IsNullOrEmpty(language_state))
            {
                mState = new StateOfRequest("LangNo");
            }
            else
            {
                mState = JsonUtil.GetObjectFromJsonString<StateOfRequest>(language_state);
            }
            mState.StartTime = DateTime.Now;

            // Added on 2018-11-24
            // start from first page
            mState.CurrentPageNo = 1;
            List<Language> languagesTemp = _languagesManager.GetOnePageOfLanguages(mState);
            string temp_state = JsonUtil.SetJsonStringFromObject(mState);
            ViewBag.LanguageState = temp_state;

            return View(nameof(LanguagesList), languagesTemp);

            /*
            int orgId = 0;
            if (mState.OrgId == 0)
            {
                // no language found or selected in this page
                // then use the first language of this page
                orgId = mState.FirstId;
            }
            else
            {
                orgId = mState.OrgId;
            }

            if (orgId != 0)
            {
                Language language = new Language();
                List<Language> languagesTemp = await _languagesManager.FindOnePageOfLanguagesForOneLanguage(mState, language, orgId);
                string temp_state = JsonUtil.SetJsonStringFromObject(mState);

                ViewBag.LanguageState = temp_state;
                return View(nameof(LanguagesList), languagesTemp);
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
