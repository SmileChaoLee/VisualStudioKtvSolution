using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VodManageSystem.Models.DataModels;

namespace VodManageSystem.Models.Dao
{
    public class LanguageManager : IDisposable
    {
        private readonly int pageSize = 18; 
        private readonly KtvSystemDBContext _context;
        /// <summary>
        /// Initializes a new instance of the <see cref="T:VodManageSystem.Models.Dao.LanguageManager"/> class.
        /// </summary>
        /// <param name="context">Context.</param>
        public LanguageManager(KtvSystemDBContext context)
        {
            _context = context;
        }

        // private methods

        // end of private methods

        // public methods

        /// <summary>
        /// Gets the dictionary of languages.
        /// </summary>
        /// <returns>The dictionary of languages.</returns>
        /// <param name="languageState">Language state.</param>
        public async Task<SortedDictionary<int, Language>> GetDictionaryOfLanguages(LanguageStateOfRequest languageState)
        {
            if (languageState == null)
            {
                return new SortedDictionary<int, Language>();
            }

            List<Language> totalLanguages = await _context.Language.AsNoTracking().ToListAsync();

            Dictionary<int, Language> languagesDictionary = null;

            if (languageState.OrderBy == "LangNo")
            {
                languagesDictionary = totalLanguages.OrderBy(x => x.LangNo)
                            .Select((m, index) => new { rowNumber = index + 1, m })
                            .ToDictionary(m => m.rowNumber, m => m.m);
            }
            else if (languageState.OrderBy == "LangNa")
            {
                languagesDictionary = totalLanguages.OrderBy(x => x.LangNa).ThenBy(x => x.LangNo)
                            .Select((m, index) => new { rowNumber = index + 1, m })
                            .ToDictionary(m => m.rowNumber, m => m.m);
            }
            else
            {
                // not inside range of roder by
                languagesDictionary = new Dictionary<int, Language>();    // empty lsit
            }

            return new SortedDictionary<int, Language>(languagesDictionary);
        }

        /// <summary>
        /// Gets the select list from a SortedDictionary of languages.
        /// </summary>
        /// <returns>The select list of languages.</returns>
        /// <param name="languageState">Language state.</param>
        public async Task<List<SelectListItem>> GetSelectListOfLanguages(LanguageStateOfRequest languageState)
        {
            List<SelectListItem> selectList = new List<SelectListItem>();
            SortedDictionary<int, Language> languageDict = await GetDictionaryOfLanguages(languageState);
            foreach (Language lang in languageDict.Values)
            {
                selectList.Add(new SelectListItem
                {
                    Text = lang.LangNa,
                    Value = lang.LangNo
                });
            }
            return selectList;
        }

        /// <summary>
        /// Gets the total page of language table.
        /// </summary>
        /// <returns>The total page of language table.</returns>
        public async Task<int> GetTotalPageOfLanguageTable()    // by condition
        {
            // have to define queryCondition
            // queryCondition has not been used for now

            int count = await _context.Language.CountAsync();
            int totalPages = count / pageSize;
            if ((totalPages * pageSize) != count)
            {
                totalPages++;
            }
            return totalPages;
        }

        /// <summary>
        /// Gets the one page of languages dictionary.
        /// </summary>
        /// <returns>The one page of languages dictionary.</returns>
        /// <param name="languageState">Language state.</param>
        public async Task<List<Language>> GetOnePageOfLanguagesDictionary(LanguageStateOfRequest languageState)
        {
            if (languageState == null)
            {
                return new List<Language>();
            }
            if (string.IsNullOrEmpty(languageState.OrderBy))
            {
                languageState.OrderBy = "LangNo";
            }

            int pageNo = languageState.CurrentPageNo;
            if (pageNo < 1)
            {
                pageNo = 1;
            }

            SortedDictionary<int, Language> languagesDictionary = await GetDictionaryOfLanguages(languageState);

            int totalCount = languagesDictionary.Count;
            int totalPages = totalCount / pageSize;
            if ((totalPages * pageSize) < totalCount)
            {
                totalPages++;
            }
            if (pageNo > totalPages)
            {
                pageNo = totalPages;
            }

            int recordNo = (pageNo - 1) * pageSize;
            List<Language> languages = languagesDictionary.Skip(recordNo).Take(pageSize).Select(m => m.Value).ToList();

            languageState.CurrentPageNo = pageNo;
            Language firstLanguage = languages.FirstOrDefault();
            if (firstLanguage != null)
            {
                languageState.OrgId = firstLanguage.Id;
                languageState.OrgLangNo = firstLanguage.LangNo;
                languageState.FirstLangId = firstLanguage.Id;
            }
            else
            {
                languageState.OrgId = 0;
                languageState.OrgLangNo = "";
                languageState.FirstLangId = 0;
            }

            return languages;

        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~LanguageManager() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
