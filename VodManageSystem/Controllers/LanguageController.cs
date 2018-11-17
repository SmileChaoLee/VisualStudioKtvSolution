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
    public class LanguageController : Controller
    {
        private readonly KtvSystemDBContext _context;
        private readonly LanguageManager _languageManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:VodManageSystem.Controllers.LanguageController"/> class.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="languageManager">Language manager.</param>
        public LanguageController(KtvSystemDBContext context, LanguageManager languageManager)
        {
            _context = context;
            _languageManager = languageManager;
        }

        // GET: /<controller>/
        public IActionResult Index(string language_state)
        {
            // new Index.cshtml
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View();

            LanguageStateOfRequest languageState = JsonUtil.GetObjectFromJsonString<LanguageStateOfRequest>(language_state);
            string temp_state = JsonUtil.SetJsonStringFromObject(languageState);
            // go to languages management main menu
            return RedirectToAction(nameof(LanguagesList), new { language_state = temp_state });    // (action, parameters)
        }

        // Get: get method
        [HttpGet, ActionName("LanguagesList")]
        public async Task<IActionResult> LanguagesList(string language_state)
        {
            LanguageStateOfRequest languageState = JsonUtil.GetObjectFromJsonString<LanguageStateOfRequest>(language_state);
            // List<Language> languages = await _languageManager.GetOnePageOfLanguagesDictionary(languageState);
            List<Language> languages = await _languageManager.GetOnePageOfLanguages(languageState);

            ViewBag.LanguageState = JsonUtil.SetJsonStringFromObject(languageState);
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
        public async Task<IActionResult> Find(string lang_no, string lang_na, string search_type, string submitbutton, string language_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            LanguageStateOfRequest languageState = JsonUtil.GetObjectFromJsonString<LanguageStateOfRequest>(language_state);
            languageState.StartTime = DateTime.Now;
            string temp_state = JsonUtil.SetJsonStringFromObject(languageState);

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
                temp_state = JsonUtil.SetJsonStringFromObject(languageState);
                return RedirectToAction(nameof(LanguagesList), new { language_state = temp_state });
            }

            string searchType = search_type.Trim().ToUpper();
            Language language = new Language(); // new object
            if (searchType == "LANGUAGE_NO")
            {
                // find one language by lang_no
                languageState.OrderBy = "LangNo";
                language.LangNo = lang_no;
            }
            else if (searchType == "LANGUAGE_NA")
            {
                // find one language by lang_na
                languageState.OrderBy = "LangNa";
                language.LangNa = lang_na;
            }
            else
            {
                // search_type not defined
                return View();
            }

            List<Language> languagesTemp = await _languageManager.FindOnePageOfLanguagesForOneLanguage(languageState, language, 0);
            temp_state = JsonUtil.SetJsonStringFromObject(languageState);

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

            LanguageStateOfRequest languageState = JsonUtil.GetObjectFromJsonString<LanguageStateOfRequest>(language_state);
            languageState.CurrentPageNo = 1;    // go to first page
            languageState.StartTime = DateTime.Now;

            string temp_state = JsonUtil.SetJsonStringFromObject(languageState);
            return RedirectToAction(nameof(LanguagesList), new { language_state = temp_state });
        }

        // Get: mothed
        [HttpGet, ActionName("LastPage")]
        public IActionResult LastPage(string language_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            LanguageStateOfRequest languageState = JsonUtil.GetObjectFromJsonString<LanguageStateOfRequest>(language_state);
            languageState.StartTime = DateTime.Now;
            // languageState.CurrentPageNo = Int32.MaxValue / languageState.PageSize;  // default value for View
            languageState.CurrentPageNo = -1;   // present last page
            string temp_state = JsonUtil.SetJsonStringFromObject(languageState);

            return RedirectToAction(nameof(LanguagesList), new { language_state = temp_state });
        }

        // Get: mothed
        [HttpGet, ActionName("PreviousPage")]
        public IActionResult PreviousPage(string language_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            LanguageStateOfRequest languageState = JsonUtil.GetObjectFromJsonString<LanguageStateOfRequest>(language_state);
            languageState.StartTime = DateTime.Now;
            languageState.CurrentPageNo--;    // go to previous page
            string temp_state = JsonUtil.SetJsonStringFromObject(languageState);

            return RedirectToAction(nameof(LanguagesList), new { language_state = temp_state });
        }

        // Get: mothed
        [HttpGet, ActionName("NextPage")]
        public IActionResult NextPage(string language_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            LanguageStateOfRequest languageState = JsonUtil.GetObjectFromJsonString<LanguageStateOfRequest>(language_state);
            languageState.StartTime = DateTime.Now;
            languageState.CurrentPageNo++;    // go to next page
            string temp_state = JsonUtil.SetJsonStringFromObject(languageState);

            return RedirectToAction(nameof(LanguagesList), new { language_state = temp_state });
        }

        // GET: Language/Add
        // the view of adding languages to Language table
        public IActionResult Add(string language_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            Language language = new Language(); // create a new Language object

            ViewBag.LanguageState = language_state; // pass the Json string to View
            return View(language);
        }

        // POST: Language/Add
        // Adds a language to Language table
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(string submitbutton, string language_state, [Bind("Id", "LangNo, LangNa,LangEn")] Language language)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            LanguageStateOfRequest languageState = JsonUtil.GetObjectFromJsonString<LanguageStateOfRequest>(language_state);
            languageState.StartTime = DateTime.Now;
            string temp_state = JsonUtil.SetJsonStringFromObject(languageState);

            int orgId = languageState.OrgId;
            string sButton = submitbutton.ToUpper();
            if (sButton == "CANCEL")
            {
                Language newLanguage = new Language();
                List<Language> languagesTemp = await _languageManager.FindOnePageOfLanguagesForOneLanguage(languageState, newLanguage, orgId);
                temp_state = JsonUtil.SetJsonStringFromObject(languageState);

                ViewBag.LanguageState = temp_state;
                return View(nameof(LanguagesList), languagesTemp);
            }
            if (ModelState.IsValid)
            {
                int result = await _languageManager.AddOneLanguageToTable(language);
                if (result == ErrorCodeModel.Succeeded)
                {
                    // succeeded to add the language
                    languageState.OrgId = language.Id;
                    languageState.OrgLangNo = language.LangNo;

                    temp_state = JsonUtil.SetJsonStringFromObject(languageState);
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

            LanguageStateOfRequest languageState = JsonUtil.GetObjectFromJsonString<LanguageStateOfRequest>(language_state);

            int id = languageState.OrgId;
            Language language = await _languageManager.FindOneLanguageById(id);

            if (language == null)
            {
                // go to previous view (List view)
                return Redirect(HttpContext.Request.Headers["Referer"]);
            }
            else
            {
                languageState.OrgId = language.Id;
                languageState.OrgLangNo = language.LangNo;
                string temp_state = JsonUtil.SetJsonStringFromObject(languageState);

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

            LanguageStateOfRequest languageState = JsonUtil.GetObjectFromJsonString<LanguageStateOfRequest>(language_state);
            languageState.StartTime = DateTime.Now;
            string temp_state = JsonUtil.SetJsonStringFromObject(languageState);

            int orgId = languageState.OrgId;    // = language.Id
            string sButton = submitbutton.ToUpper();
            if (sButton == "CANCEL")
            {
                temp_state = JsonUtil.SetJsonStringFromObject(languageState);
                return RedirectToAction(nameof(LanguagesList), new { language_state = temp_state });
            }
            if (ModelState.IsValid)
            {
                // start updating table
                int result = await _languageManager.UpdateOneLanguageById(orgId, language);
                if (result == ErrorCodeModel.Succeeded)
                {
                    // succeeded to update
                    Language newLanguage = new Language();
                    List<Language> languagesTemp = await _languageManager.FindOnePageOfLanguagesForOneLanguage(languageState, newLanguage, orgId);
                    temp_state = JsonUtil.SetJsonStringFromObject(languageState);

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

            LanguageStateOfRequest languageState = JsonUtil.GetObjectFromJsonString<LanguageStateOfRequest>(language_state);
            int id = languageState.OrgId;
            Language language = await _languageManager.FindOneLanguageById(id);

            if (language == null)
            {
                // go to previous view (List view)
                return Redirect(HttpContext.Request.Headers["Referer"]);
            }
            else
            {
                languageState.OrgId = id;
                languageState.OrgLangNo = language.LangNo;
                string temp_state = JsonUtil.SetJsonStringFromObject(languageState);

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

            LanguageStateOfRequest languageState = JsonUtil.GetObjectFromJsonString<LanguageStateOfRequest>(language_state);
            languageState.StartTime = DateTime.Now;
            string temp_state = JsonUtil.SetJsonStringFromObject(languageState);

            int orgId = languageState.OrgId;
            string sButton = submitbutton.ToUpper();
            if (sButton == "CANCEL")
            {
                temp_state = JsonUtil.SetJsonStringFromObject(languageState);
                return RedirectToAction(nameof(LanguagesList), new { language_state = temp_state });
            }

            if (ModelState.IsValid)
            {
                // start deleting the language from table
                int result = await _languageManager.DeleteOneLanguageById(orgId);
                if (result == ErrorCodeModel.Succeeded)
                {
                    // succeeded to delete a language
                    List<Language> languagesTemp = await _languageManager.FindOnePageOfLanguagesForOneLanguage(languageState, language, 0);
                    temp_state = JsonUtil.SetJsonStringFromObject(languageState);

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

            LanguageStateOfRequest languageState = JsonUtil.GetObjectFromJsonString<LanguageStateOfRequest>(language_state);
            Language language = await _languageManager.FindOneLanguageById(languageState.OrgId);

            if (language == null)
            {
                // go to previous view (List view)
                return Redirect(HttpContext.Request.Headers["Referer"]);
            }
            else
            {
                languageState.OrgId = language.Id;
                languageState.OrgLangNo = language.LangNo;
                string temp_state = JsonUtil.SetJsonStringFromObject(languageState);

                ViewBag.LanguageState = temp_state;
                return View(language);
            }
        }

        // POST:
        [HttpPost, ActionName("Details")]
        public async Task<IActionResult> DetailsReturn(string language_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            LanguageStateOfRequest languageState = JsonUtil.GetObjectFromJsonString<LanguageStateOfRequest>(language_state);
            languageState.StartTime = DateTime.Now;

            int orgId = languageState.OrgId;
            Language language = new Language();
            List<Language> languagesTemp = await _languageManager.FindOnePageOfLanguagesForOneLanguage(languageState, language, orgId);
            string temp_state = JsonUtil.SetJsonStringFromObject(languageState);

            ViewBag.LanguageState = temp_state;
            return View(nameof(LanguagesList), languagesTemp);
        }

        // Get:
        [HttpGet, ActionName("ChangeOrder")]
        public async Task<IActionResult> ChangeOrder(string language_state)
        {
            if (!LoginUtil.CheckIfLoggedIn(HttpContext)) return View(nameof(Index));

            LanguageStateOfRequest languageState = JsonUtil.GetObjectFromJsonString<LanguageStateOfRequest>(language_state);
            languageState.StartTime = DateTime.Now;

            int orgId = 0;
            if (languageState.OrgId == 0)
            {
                // no language found or selected in this page
                // then use the first language of this page
                orgId = languageState.FirstId;
            }
            else
            {
                orgId = languageState.OrgId;
            }

            if (orgId != 0)
            {
                Language language = new Language();
                List<Language> languagesTemp = await _languageManager.FindOnePageOfLanguagesForOneLanguage(languageState, language, orgId);
                string temp_state = JsonUtil.SetJsonStringFromObject(languageState);

                ViewBag.LanguageState = temp_state;
                return View(nameof(LanguagesList), languagesTemp);
            }
            else
            {
                // return to the previous page
                return Redirect(HttpContext.Request.Headers["Referer"]);
            }
        }
    }
}
