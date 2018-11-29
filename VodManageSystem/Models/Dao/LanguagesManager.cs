using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VodManageSystem.Models.DataModels;

namespace VodManageSystem.Models.Dao
{
    public class LanguagesManager : IDisposable
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
        public LanguagesManager(KtvSystemDBContext context)
        {
            _context = context;
        }

        // private methods

        /// <summary>
        /// Gets the total page of language table.
        /// </summary>
        /// <returns>The total page of language table.</returns>
        private int[] GetTotalRecordsAndPages(int pageSize)    // by condition
        {
            int[] result = new int[2] { 0, 0 };

            if (pageSize <= 0)
            {
                Console.WriteLine("The value of pageSize cannot be less than 0.");
                return result;
            }
            // have to define queryCondition
            // queryCondition has not been used for now

            int count = _context.Language.Count();
            int totalPages = count / pageSize;
            if ((totalPages * pageSize) != count)
            {
                totalPages++;
            }

            result[0] = count;
            result[1] = totalPages;

            return result;
        }

        void UpdateStateOfRequest(StateOfRequest mState, Language firstLanguage, int pageNo, int pageSize, int totalRecords, int totalPages, bool isFind = false)
        {
            mState.CurrentPageNo = pageNo;
            mState.PageSize = pageSize;
            mState.TotalRecords = totalRecords;
            mState.TotalPages = totalPages;
            if (firstLanguage != null)
            {
                mState.FirstId = firstLanguage.Id;
                if (!isFind)
                {
                    // mState.OrgId = mState.FirstId;
                }
            }
            else
            {
                mState.OrgId = 0;
                mState.OrgNo = "";
                mState.FirstId = 0;
            }
        }

        private IQueryable<Language> GetAllLanguagesIQueryable(StateOfRequest mState)
        {
            if (mState == null)
            {
                return null;
            }
            int pageSize = mState.PageSize;
            if (pageSize <= 0)
            {
                Console.WriteLine("The value of pageSize cannot be less than 0.");
                return null;
            }

            IQueryable<Language> totalLanguages = _context.Language;

            IQueryable<Language> languages;

            string orderByParam = mState.OrderBy.Trim();
            if (orderByParam == "")
            {
                languages = totalLanguages;
            }
            else if (orderByParam.Equals("LangNo", StringComparison.OrdinalIgnoreCase))
            {
                languages = totalLanguages.OrderBy(x => x.LangNo);
            }
            else if (orderByParam.Equals("LangNa", StringComparison.OrdinalIgnoreCase))
            {
                languages = totalLanguages.OrderBy(x => x.LangNa);
            }
            else
            {
                // not inside range of roder by
                languages = null;   // empty lsit
            }

            return languages;
        }

        // end of private methods

        // public methods
        public List<Language> GetAllLanguages(StateOfRequest mState)
        {
            if (mState == null)
            {
                return new List<Language>();    // return empty list
            }
            int pageSize = mState.PageSize;
            if (pageSize <= 0)
            {
                Console.WriteLine("The value of pageSize cannot be less than 0.");
                return new List<Language>();
            }

            mState.CurrentPageNo = -100;  // represnt to get all languages
            List<Language> totalLanguages = GetOnePageOfLanguages(mState);

            return totalLanguages;
        }

        public List<Language> GetOnePageOfLanguages(StateOfRequest mState)
        {
            if (mState == null)
            {
                return new List<Language>();
            }
            int pageSize = mState.PageSize;
            if (pageSize <= 0)
            {
                Console.WriteLine("The value of pageSize cannot be less than 0.");
                return new List<Language>();
            }

            IQueryable<Language> totalLanguages = GetAllLanguagesIQueryable(mState);
            if (totalLanguages == null)
            {
                return new List<Language>();
            }

            int pageNo = mState.CurrentPageNo;
            int[] returnNumbers = GetTotalRecordsAndPages(pageSize);
            int totalRecords = returnNumbers[0];
            int totalPages = returnNumbers[1];

            if (pageNo == -1)
            {
                // get the last page
                pageNo = totalPages;
            }
            else if (pageNo == -100)
            {
                // get all languages
                pageNo = 1; // restore pageNo to 1
                pageSize = totalRecords;
                totalPages = 1;
            }
            else
            {
                if (pageNo < 1)
                {
                    pageNo = 1;
                }
                else if (pageNo > totalPages)
                {
                    pageNo = totalPages;
                }
            }

            int recordNum = (pageNo - 1) * pageSize;

            List<Language> languages = totalLanguages.Skip(recordNum).Take(pageSize).ToList();

            UpdateStateOfRequest(mState, languages.FirstOrDefault(), pageNo, pageSize, totalRecords, totalPages);

            return languages;
        }

        /// <summary>
        /// Gets the select list from a SortedDictionary of languages.
        /// </summary>
        /// <returns>The select list of languages.</returns>
        /// <param name="mState">Language state.</param>
        public List<SelectListItem> GetSelectListOfLanguages(StateOfRequest mState)
        {
            List<SelectListItem> selectList = new List<SelectListItem>();
            List<Language> languages = GetAllLanguages(mState);
            foreach (Language lang in languages)
            {
                selectList.Add(new SelectListItem
                {
                    Text = lang.LangNa,
                    Value = Convert.ToString(lang.Id)
                });
            }
            return selectList;
        }

        /// <summary>
        /// Finds the one page of languages for one language.
        /// </summary>
        /// <returns>The one page of languages for one language.</returns>
        /// <param name="mState">Language state.</param>
        /// <param name="language">Language.</param>
        /// <param name="id">Identifier.</param>
        public List<Language> FindOnePageOfLanguagesForOneLanguage(StateOfRequest mState, Language language, int id)
        {
            if ( (mState == null) || (language == null) )
            {
                return new List<Language>();
            }
            int pageSize = mState.PageSize;
            if (pageSize <= 0)
            {
                Console.WriteLine("The value of pageSize cannot be less than 0.");
                return new List<Language>();
            }

            IQueryable<Language> totalLanguages = GetAllLanguagesIQueryable(mState);
            if (totalLanguages == null)
            {
                return new List<Language>();
            }

            List<Language> languages = null;
            Language languageWithIndex = null;
            IQueryable<Language> languagesTempList = null;

            string orderByParam = mState.OrderBy.Trim();
            if (id >= 0)
            {
                // There was a language selected
                languagesTempList = totalLanguages.Where(x => x.Id == id);
            }
            else
            {
                // No language selected
                if (orderByParam == "")
                {
                    // order by Id
                    int lang_id = language.Id;
                    languagesTempList = totalLanguages.Where(x => (x.Id == lang_id));
                }
                else if (orderByParam.Equals("LangNo", StringComparison.OrdinalIgnoreCase))
                {
                    string lang_no = language.LangNo.Trim();
                    int len = lang_no.Length;
                    languagesTempList = totalLanguages.Where(x => x.LangNo.Trim().Substring(0, len) == lang_no);
                }
                else if (orderByParam.Equals("LangNa", StringComparison.OrdinalIgnoreCase))
                {
                    string lang_na = language.LangNa.Trim();
                    int len = lang_na.Length;
                    languagesTempList = totalLanguages.Where(x => x.LangNa.Trim().Substring(0, len) == lang_na);
                }
                else
                {
                    // not inside range of roder by then return empty lsit
                    return new List<Language>();
                }
            }

            int totalRecords = totalLanguages.Count();  // the whole language table

            bool isFound = true;
            languageWithIndex = languagesTempList.FirstOrDefault(); // the first one found
            if (languageWithIndex == null)
            {
                isFound = false;    // language that was assigned is not found
                if (totalRecords == 0)
                {
                    // Language Table is empty
                    UpdateStateOfRequest(mState, languageWithIndex, mState.CurrentPageNo, pageSize, 0, 0, true);
                    // return empty list
                    return new List<Language>();
                }
                else
                {
                    // go to last page
                    languageWithIndex = totalLanguages.LastOrDefault();
                }
            }

            language.CopyFrom(languageWithIndex);

            // find the row number of languageWithIndex
            int tempCount = 0;
            foreach (var languageVar in totalLanguages)
            {
                ++tempCount;    // first row number is 1
                if (languageVar.Id == languageWithIndex.Id)
                {
                    break;
                }
            }
            int pageNo = tempCount / pageSize;
            if ((pageNo * pageSize) != tempCount)
            {
                pageNo++;
            }

            int recordNo = (pageNo - 1) * pageSize;

            languages = totalLanguages.Skip(recordNo).Take(pageSize).ToList();

            int totalPages = totalRecords / pageSize;
            if ((totalPages * pageSize) != totalRecords)
            {
                totalPages++;
            }

            if (isFound)
            {
                // found
                mState.OrgId = language.Id; // chnaged OrgId to the language id found
            }
            else
            {
                // not found, then it is last page and last record
                mState.OrgId = 0;   // no language is selected
            }
            UpdateStateOfRequest(mState, languages.FirstOrDefault(), pageNo, pageSize, totalRecords, totalPages, true);

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
                // language no is duplicate
                result = ErrorCodeModel.LanguageNoDuplicate;
                return result;
            }

            using (var dbTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    _context.Add(language);
                    await _context.SaveChangesAsync();
                    dbTransaction.Commit();
                    result = ErrorCodeModel.Succeeded;
                }
                catch (DbUpdateException ex)
                {
                    string errorMsg = ex.ToString();
                    Console.WriteLine("Failed to add one language: \n" + errorMsg);
                    dbTransaction.Rollback();
                    result = ErrorCodeModel.DatabaseError;
                }
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
                    using (var dbTransaction = _context.Database.BeginTransaction())
                    {
                        try
                        {
                            await _context.SaveChangesAsync();
                            dbTransaction.Commit();
                            result = ErrorCodeModel.Succeeded; // succeeded to update
                        }
                        catch (DbUpdateException ex)
                        {
                            string msg = ex.ToString();
                            Console.WriteLine("Failed to update language table: \n" + msg);
                            dbTransaction.Rollback();
                            result = ErrorCodeModel.DatabaseError;
                        }
                    }
                }
                else
                {
                    result = ErrorCodeModel.LanguageNotChanged; // no changed
                }
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

            Language orgLanguage = await FindOneLanguageByLangNo(lang_no);
            if (orgLanguage == null)
            {
                // the original language does not exist any more
                result = ErrorCodeModel.OriginalLanguageNotExist;
            }
            else
            {
                using (var dbTransaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        _context.Language.Remove(orgLanguage);
                        await _context.SaveChangesAsync();
                        dbTransaction.Commit();
                        result = ErrorCodeModel.Succeeded; // succeeded to update
                    }
                    catch (DbUpdateException ex)
                    {
                        string msg = ex.ToString();
                        Console.WriteLine("Failed to delete one language. Please see log file.\n" + msg);
                        dbTransaction.Rollback();
                        result = ErrorCodeModel.DatabaseError;
                    }
                }
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

            Language orgLanguage = await FindOneLanguageById(id);
            if (orgLanguage == null)
            {
                // the original language does not exist any more
                result = ErrorCodeModel.OriginalLanguageNotExist;
            }
            else
            {
                using (var dbTransaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        _context.Language.Remove(orgLanguage);
                        await _context.SaveChangesAsync();
                        dbTransaction.Commit();
                        result = ErrorCodeModel.Succeeded; // succeeded to update
                    }
                    catch (DbUpdateException ex)
                    {
                        string msg = ex.ToString();
                        Console.WriteLine("Failed to delete one language. Please see log file.\n" + msg);
                        dbTransaction.Rollback();
                        result = ErrorCodeModel.DatabaseError;
                    }
                }
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
