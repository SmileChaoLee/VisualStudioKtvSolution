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
        // private properties
        private readonly KtvSystemDBContext _context;
        // end of private properties

        // public properties
        // end of public properties

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
        public async Task<List<Language>> GetAllLanguagesAsync(LanguageStateOfRequest languageState)
        {
            List<Language> totalLanguages;
            if (languageState.OrderBy == "LangNo")
            {
                totalLanguages = await _context.Language
                                          .OrderBy(x => x.LangNo)
                                          .AsNoTracking().ToListAsync();
            }
            else if (languageState.OrderBy == "LangNa")
            {
                totalLanguages = await _context.Language
                                          .OrderBy(x => x.LangNo).ThenBy(x => x.LangNo)
                                          .AsNoTracking().ToListAsync();
            }
            else
            {
                totalLanguages = new List<Language>();
            }
            return totalLanguages;
        }

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
        public async Task<int> GetTotalPageOfLanguageTable(int pageSize)    // by condition
        {
            if (pageSize <= 0)
            {
                Console.WriteLine("The value of pageSize cannot be less than 0.");
                return 0;
            }
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

            SortedDictionary<int, Language> languagesDictionary = await GetDictionaryOfLanguages(languageState);

            int pageNo = languageState.CurrentPageNo;
            int pageSize = languageState.PageSize;
            int totalCount = languagesDictionary.Count;
            int totalPages = totalCount / pageSize;
            if ((totalPages * pageSize) < totalCount)
            {
                totalPages++;
            }

            if (pageNo == -1)
            {
                // get the last page
                pageNo = totalPages;
            }
            else if (pageNo < 1)
            {
                pageNo = 1;
            }
            else if (pageNo > totalPages)
            {
                pageNo = totalPages;
            }

            int recordNo = (pageNo - 1) * pageSize;
            List<Language> languages = languagesDictionary.Skip(recordNo).Take(pageSize).Select(m => m.Value).ToList();

            languageState.CurrentPageNo = pageNo;
            Language firstLanguage = languages.FirstOrDefault();
            if (firstLanguage != null)
            {
                languageState.FirstId = firstLanguage.Id;
            }
            else
            {
                languageState.OrgId = 0;
                languageState.OrgLangNo = "";
                languageState.FirstId = 0;
            }

            return languages;
        }

        public async Task<List<Language>> GetOnePageOfLanguages(LanguageStateOfRequest languageState)
        {
            if (languageState == null)
            {
                return new List<Language>();
            }
            if (string.IsNullOrEmpty(languageState.OrderBy))
            {
                // default is order by language's No
                languageState.OrderBy = "LangNo";
            }

            int pageNo = languageState.CurrentPageNo;
            int pageSize = languageState.PageSize;
            int totalPages = await GetTotalPageOfLanguageTable(pageSize);
            if (pageNo == -1)
            {
                // get the last page
                pageNo = totalPages;
            }
            else if (pageNo < 1)
            {
                pageNo = 1;
            }
            else if (pageNo > totalPages)
            {
                pageNo = totalPages;
            }

            int recordNum = (pageNo - 1) * pageSize;

            List<Language> languages;

            if (languageState.OrderBy == "LangNo")
            {
                languages = await _context.Language.OrderBy(x => x.LangNo)
                                        .Skip(recordNum).Take(pageSize)
                                        .AsNoTracking().ToListAsync();
            }
            else if (languageState.OrderBy == "LangNa")
            {
                languages = await _context.Language.OrderBy(x => x.LangNa).ThenBy(x => x.LangNo)
                                        .Skip(recordNum).Take(pageSize)
                                        .AsNoTracking().ToListAsync();
            }
            else
            {
                languages = await _context.Language
                                        .Skip(recordNum).Take(pageSize)
                                        .AsNoTracking().ToListAsync();
            }

            languageState.CurrentPageNo = pageNo;
            Language firstLanguage = languages.FirstOrDefault();
            if (firstLanguage != null)
            {
                languageState.FirstId = firstLanguage.Id;
            }
            else
            {
                languageState.OrgId = 0;
                languageState.OrgLangNo = "";
                languageState.FirstId = 0;
            }

            return languages;
        }

        /// <summary>
        /// Finds the one page of languages for one language.
        /// </summary>
        /// <returns>The one page of languages for one language.</returns>
        /// <param name="languageState">Language state.</param>
        /// <param name="language">Language.</param>
        /// <param name="id">Identifier.</param>
        public async Task<List<Language>> FindOnePageOfLanguagesForOneLanguage(LanguageStateOfRequest languageState, Language language, int id)
        {
            if ( (languageState == null) || (language == null) )
            {
                return new List<Language>();
            }
            if (string.IsNullOrEmpty(languageState.OrderBy))
            {
                languageState.OrderBy = "LangNo";
            }

            int pageSize = languageState.PageSize;

            List<Language> languages = null;
            KeyValuePair<int,Language> languageWithIndex = new KeyValuePair<int, Language>(-1,null);

            SortedDictionary<int, Language> languagesDictionary = await GetDictionaryOfLanguages(languageState);

            if (id > 0)
            {
                // There was a selected language
                languageWithIndex = languagesDictionary.Where(x=>x.Value.Id == id).SingleOrDefault();
            }
            else
            {
                // No selected anguage
                if (languageState.OrderBy == "LangNo")
                {
                    string lang_no = language.LangNo;
                    languageWithIndex = languagesDictionary.Where(x=>(String.Compare(x.Value.LangNo,lang_no)>=0)).FirstOrDefault();
                }
                else if (languageState.OrderBy == "LangNa")
                {
                    string lang_na = language.LangNa;
                    languageWithIndex = languagesDictionary.Where(x=>(String.Compare(x.Value.LangNa,lang_na)>=0)).FirstOrDefault();
                }
                else
                {
                    // not inside range of roder by then return empty lsit
                    return new List<Language>(); 
                }
            }

            if (languageWithIndex.Value == null)
            {
                if (languagesDictionary.Count == 0)
                {
                    // dictionary (Language Table) is empty
                    languageState.OrgId = 0;
                    languageState.OrgLangNo = "";
                    languageState.FirstId = 0;
                    // return empty list
                    return new List<Language>();
                }
                else
                {
                    // go to last page
                    languageWithIndex = languagesDictionary.LastOrDefault();
                }
            }
        
            Language languageFound = languageWithIndex.Value;
            language.CopyFrom(languageFound);

            int tempCount = languageWithIndex.Key;
            int pageNo =  tempCount / pageSize;
            if ( (pageNo * pageSize) != tempCount)
            {
                pageNo++;
            }
            int recordNo = (pageNo - 1) * pageSize;
            languages = languagesDictionary.Skip(recordNo).Take(pageSize).Select(m=>m.Value).ToList();

            languageState.CurrentPageNo = pageNo;
            languageState.OrgId = language.Id;
            languageState.OrgLangNo = language.LangNo;

            Language firstLanguage = languages.FirstOrDefault();
            if(firstLanguage != null)
            {
                languageState.FirstId = firstLanguage.Id;
            }
            else
            {
                languageState.FirstId = 0;
            }
               
            return languages;
        }

        /// <summary>
        /// Finds the one language by language no.
        /// </summary>
        /// <returns>The one language by language no.</returns>
        /// <param name="lang_no">Language no.</param>
        public async Task<Language> FindOneLanguageByLangNo(string lang_no)
        {
            Language language = await _context.Language.Where(x=>x.LangNo == lang_no).SingleOrDefaultAsync();

            return language;
        }

        /// <summary>
        /// Finds the one language by identifier.
        /// </summary>
        /// <returns>The one language by identifier (Language.Id).</returns>
        /// <param name="id">the id of the language.</param>
        public async Task<Language> FindOneLanguageById(int id)
        {
            Language language = await _context.Language.Where(x=>x.Id == id).SingleOrDefaultAsync();

            return language;
        }

        /// <summary>
        /// Adds the one language to table.
        /// </summary>
        /// <returns>Return the error code.</returns>
        /// <param name="language">Language.</param>
        public async Task<int> AddOneLanguageToTable(Language language)
        {
            int result = ErrorCodeModel.ErrorBecauseBugs;
            if (language == null)
            {
                // the data for updating is empty
                result = ErrorCodeModel.LanguageIsNull;
                return result;
            }
            if (string.IsNullOrEmpty(language.LangNo))
            {
                // the language no that input by user is empty
                result = ErrorCodeModel.LanguageNoIsEmpty;
                return result;
            }
            Language oldLanguage = await FindOneLanguageByLangNo(language.LangNo);
            if (oldLanguage != null)
            {
                // language_no is duplicate
                result = ErrorCodeModel.LanguageNoDuplicate;
                return result;
            }

            try
            {
                _context.Add(language);
                await _context.SaveChangesAsync();
                result = ErrorCodeModel.Succeeded;
            }
            catch (DbUpdateException ex)
            {
                string errorMsg = ex.ToString();
                Console.WriteLine("Failed to add one language: \n" + errorMsg);
                result = ErrorCodeModel.DatabaseError;    
            }

            return result;
        }

        /// <summary>
        /// Updates the one language by identifier.
        /// </summary>
        /// <returns>Return the error code</returns>
        /// <param name="id">Identifier.</param>
        /// <param name="language">Language.</param>
        public async Task<int> UpdateOneLanguageById(int id, Language language)
        {
            int result = ErrorCodeModel.ErrorBecauseBugs;
            if (id == 0)
            {
                // its a bug, id of language cannot be 0
                result = ErrorCodeModel.ErrorBecauseBugs;
                return result;
            }
            if (language == null)
            {
                // the data for updating is empty
                result = ErrorCodeModel.LanguageIsNull;
                return result;
            }
            if (string.IsNullOrEmpty(language.LangNo))
            {
                // the language no that input by user is empty
                result = ErrorCodeModel.LanguageNoIsEmpty;
                return result;
            }
            Language newLanguage = await FindOneLanguageByLangNo(language.LangNo);
            if (newLanguage != null)
            {
                if (newLanguage.Id != id)
                {
                    // language no is duplicate
                    result = ErrorCodeModel.LanguageNoDuplicate;
                    return result;
                }
            }

            try
            {
                Language orgLanguage = await FindOneLanguageById(id);
                if (orgLanguage == null)
                {
                    // the original language does not exist any more
                    result = ErrorCodeModel.OriginalLanguageNotExist;
                    return result;
                }
                else
                {
                    orgLanguage.CopyColumnsFrom(language);
                    
                    // check if entry state changed
                    if ( (_context.Entry(orgLanguage).State) == EntityState.Modified)
                    {
                        await _context.SaveChangesAsync();
                        result = ErrorCodeModel.Succeeded; // succeeded to update
                    }
                    else
                    {
                        result = ErrorCodeModel.LanguageNotChanged; // no changed
                    }
                }
            }
            catch (DbUpdateException ex)
            {
                string msg = ex.ToString();
                Console.WriteLine("Failed to update language table: \n" + msg);
                result = ErrorCodeModel.DatabaseError;
            }
            return result;
        }

        /// <summary>
        /// Deletes the one language by language no.
        /// </summary>
        /// <returns>Return the error code.</returns>
        /// <param name="lang_no">Language no.</param>
        public async Task<int> DeleteOneLanguageByLangNo(string lang_no)
        {
            int result = ErrorCodeModel.ErrorBecauseBugs;
            if (string.IsNullOrEmpty(lang_no))
            {
                // its a bug, the original language no is empty
                result = ErrorCodeModel.OriginalLanguageNoIsEmpty;
                return result;
            }
            try
            {
                Language orgLanguage = await FindOneLanguageByLangNo(lang_no);
                if (orgLanguage == null)
                {
                    // the original language does not exist any more
                    result = ErrorCodeModel.OriginalLanguageNotExist;
                }
                else
                {
                    _context.Language.Remove(orgLanguage);
                    await _context.SaveChangesAsync();
                    result = ErrorCodeModel.Succeeded; // succeeded to update
                }
            }
            catch (DbUpdateException ex)
            {
                string msg = ex.ToString();
                Console.WriteLine("Failed to delete one language. Please see log file.\n" + msg);
                result = ErrorCodeModel.DatabaseError;
            }
            return result;
        }

        /// <summary>
        /// Deletes the one language by identifier.
        /// </summary>
        /// <returns>Return the error code.</returns>
        /// <param name="id">Identifier.</param>
        public async Task<int> DeleteOneLanguageById(int id)
        {
            int result = ErrorCodeModel.ErrorBecauseBugs;
            if (id == 0)
            {
                // its a bug, the id of language cannot be 0
                result = ErrorCodeModel.ErrorBecauseBugs;
                return result;
            }
            try
            {
                Language orgLanguage = await FindOneLanguageById(id);
                if (orgLanguage == null)
                {
                    // the original language does not exist any more
                    result = ErrorCodeModel.OriginalLanguageNotExist;
                }
                else
                {
                    _context.Language.Remove(orgLanguage);
                    await _context.SaveChangesAsync();
                    result = ErrorCodeModel.Succeeded; // succeeded to update
                }
            }
            catch (DbUpdateException ex)
            {
                string msg = ex.ToString();
                Console.WriteLine("Failed to delete one language. Please see log file.\n" + msg);
                result = ErrorCodeModel.DatabaseError;
            }
            return result;
        }


        // end of public methods

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
