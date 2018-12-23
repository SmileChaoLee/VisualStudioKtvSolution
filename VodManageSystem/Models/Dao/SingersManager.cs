using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VodManageSystem.Models.DataModels;

namespace VodManageSystem.Models.Dao
{
    public class SingersManager : IDisposable
    {
        // private properties
        private readonly KtvSystemDBContext _context;
        // end of private properties

        // public properties
        // end of public properties

        /// <summary>
        /// Initializes a new instance of the <see cref="T:VodManageSystem.Models.Dao.SingerManager"/> class.
        /// </summary>
        /// <param name="context">Context.</param>
        public SingersManager(KtvSystemDBContext context)
        {
            _context = context;
        }

        // private methods

        /// <summary>
        /// Verifies the singer.
        /// </summary>
        /// <returns>The singer.</returns>
        /// <param name="singer">Singer.</param>
        private async Task<int> VerifySinger(Singer singer)
        {
            // int result = 1; // valid by verification 
            int result = ErrorCodeModel.SingareaNoNotFound;
            if (singer.AreaId >= 0)
            {
                Singarea area = await _context.Singarea.Where(x => x.Id == singer.AreaId).SingleOrDefaultAsync();
                if (area != null)
                {
                    result = 1; // found singarea
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the total page of singer table.
        /// </summary>
        /// <returns>The total page of Singer table.</returns>
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

            int count = _context.Singer.Count();
            int totalPages = count / pageSize;
            if ((totalPages * pageSize) != count)
            {
                totalPages++;
            }

            result[0] = count;
            result[1] = totalPages;

            return result;
        }

        private void UpdateStateOfRequest(StateOfRequest mState, Singer firstSinger, int pageNo, int pageSize, int totalRecords, int totalPages, bool isFind = false)
        {
            mState.CurrentPageNo = pageNo;
            mState.PageSize = pageSize;
            mState.TotalRecords = totalRecords;
            mState.TotalPages = totalPages;
            if (firstSinger != null)
            {
                mState.FirstId = firstSinger.Id;
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

        private IQueryable<Singer> GetAllSingersIQueryableWithoutFilter(StateOfRequest mState)
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

            IQueryable<Singer> totalSingers = _context.Singer.Include(x => x.Singarea);

            IQueryable<Singer> singers;

            string orderByParam = mState.OrderBy.Trim();
            if (orderByParam == "")
            {
                singers = totalSingers;
            }
            else if (orderByParam.Equals("SingNo", StringComparison.OrdinalIgnoreCase))
            {
                singers = totalSingers.OrderBy(x => x.SingNo);
            }
            else if (orderByParam.Equals("SingNa", StringComparison.OrdinalIgnoreCase))
            {
                singers = totalSingers.OrderBy(x => x.SingNa).ThenBy(x => x.SingNo);
            }
            else
            {
                // not inside range of roder by
                singers = null;   // empty lsit
            }

            return singers;
        }

        private IQueryable<Singer> GetSingersIQueryableAddFilter(IQueryable<Singer> originalSingers, string filter)
        {
            IQueryable<Singer> singers = originalSingers;
            if ((originalSingers != null) && (!string.IsNullOrEmpty(filter)))
            {
                string queryString = filter.Trim();
                int plusPos = queryString.IndexOf("+", 0, StringComparison.Ordinal);
                if (plusPos >= 1)
                {
                    // the query condition has two parts
                    // the first one is the field name in singer table
                    // the second one is the vaue that the field contains
                    string fieldName = queryString.Substring(0, plusPos).Trim();
                    string fieldSubValue = queryString.Substring(plusPos + 1).Trim();
                    if (!string.IsNullOrEmpty(fieldSubValue))
                    {
                        if (fieldName.Equals("SingNo", StringComparison.OrdinalIgnoreCase))
                        {
                            singers = originalSingers.Where(x => x.SingNo.Contains(fieldSubValue));
                        }
                        else if (fieldName.Equals("SingNa", StringComparison.OrdinalIgnoreCase))
                        {
                            singers = originalSingers.Where(x => x.SingNa.Contains(fieldSubValue));
                        }
                    }
                }
            }

            return singers;
        }

        private IQueryable<Singer> GetAllSingersIQueryable(StateOfRequest mState)
        {
            IQueryable<Singer> singers = GetAllSingersIQueryableWithoutFilter(mState);
            singers = GetSingersIQueryableAddFilter(singers, mState.QueryCondition);

            return singers;
        }

        // end of private methods


        // public methods
        public List<Singer> GetAllSingers(StateOfRequest mState) {
            if (mState == null)
            {
                return new List<Singer>();  // return empty list
            }

            mState.CurrentPageNo = -100;   // present to get all singers
            List<Singer> totalSingers = GetOnePageOfSingers(mState);

            return totalSingers;
        }

        public List<Singer> GetOnePageOfSingers(StateOfRequest mState)
        {
            if (mState == null)
            {
                return new List<Singer>();
            }
            int pageSize = mState.PageSize;
            if (pageSize <= 0)
            {
                Console.WriteLine("The value of pageSize cannot be less than 0.");
                return new List<Singer>();
            }

            IQueryable<Singer> totalSingers = GetAllSingersIQueryable(mState);
            if (totalSingers == null)
            {
                return new List<Singer>();
            }

            int pageNo = mState.CurrentPageNo;
            int[] returnNumbers = GetTotalRecordsAndPages(pageSize);
            int totalRecords = returnNumbers[0];
            int totalPages = returnNumbers[1];

            /*
            int totalRecords = totalSingers.Count();
            int totalPages = totalRecords / pageSize;
            if ((totalPages * pageSize) != totalRecords)
            {
                totalPages++;
            }
            */

            if (pageNo == -1)
            {
                // get the last page
                pageNo = totalPages;
            }
            else if (pageNo == -100)
            {
                // get all singers
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

            List<Singer> singers = totalSingers.Skip(recordNum).Take(pageSize).ToList();

            UpdateStateOfRequest(mState, singers.FirstOrDefault(), pageNo, pageSize, totalRecords, totalPages);

            return singers;
        }

        public List<Singer> GetOnePageOfSingersByAreaSex(StateOfRequest mState, int areaId, string sex, bool isWebAPI)
        {
            if (mState == null)
            {
                return new List<Singer>();
            }
            int pageSize = mState.PageSize;
            if (pageSize <= 0)
            {
                Console.WriteLine("The value of pageSize cannot be less than 0.");
                return new List<Singer>();
            }

            IQueryable<Singer> totalSingers = GetAllSingersIQueryable(mState);
            if (totalSingers == null)
            {
                return new List<Singer>();
            }

            if (sex == "0")
            {
                totalSingers = totalSingers.Where(x => x.AreaId == areaId);
            }
            else
            {
                totalSingers = totalSingers.Where(x => (x.AreaId == areaId) && (x.Sex == sex)) ;
            }

            int pageNo = mState.CurrentPageNo;
            int totalRecords = totalSingers.Count();
            int totalPages = totalRecords / pageSize;
            if ((totalPages * pageSize) != totalRecords)
            {
                totalPages++;
            }

            if (pageNo == -1)
            {
                // get the last page
                pageNo = totalPages;
            }
            else if (pageNo == -100)
            {
                // get all singers
                pageNo = 1; // restore pageNo to 1
                pageSize = totalRecords;
                totalPages = 1;
            }
            else
            {
                if (!isWebAPI)
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
            }

            int recordNum = (pageNo - 1) * pageSize;

            List<Singer> singers = totalSingers.Skip(recordNum).Take(pageSize).ToList();

            UpdateStateOfRequest(mState, singers.FirstOrDefault(), pageNo, pageSize, totalRecords, totalPages);

            return singers;
        }

        /// <summary>
        /// Gets the select list from a SortedDictionary of singers.
        /// </summary>
        /// <returns>The select list of singers.</returns>
        /// <param name="mState">Singer state.</param>
        public List<SelectListItem> GetSelectListOfSingers(StateOfRequest mState)
        {
            List<SelectListItem> selectList = new List<SelectListItem>();
            List<Singer> singers = GetAllSingers(mState);
            foreach (Singer sing in singers)
            {
                selectList.Add(new SelectListItem
                {
                    Text = sing.SingNa,
                    Value = Convert.ToString(sing.Id)
                });
            }
            return selectList;
        }

        /// <summary>
        /// Finds the one page of singers for one singer.
        /// </summary>
        /// <returns>The one page of singers for one singer.</returns>
        /// <param name="mState">Singer state.</param>
        /// <param name="singer">Singer.</param>
        /// <param name="id">Identifier.</param>
        public List<Singer> FindOnePageOfSingersForOneSinger(StateOfRequest mState, Singer singer, int id)
        {
            if ((mState == null) || (singer == null))
            {
                return new List<Singer>();
            }
            int pageSize = mState.PageSize;
            if (pageSize <= 0)
            {
                Console.WriteLine("The value of pageSize cannot be less than 0.");
                return new List<Singer>();
            }

            IQueryable<Singer> totalSingers = GetAllSingersIQueryable(mState);
            if (totalSingers == null)
            {
                return new List<Singer>();
            }

            List<Singer> singers = null;
            Singer singerWithIndex = null;
            IQueryable<Singer> singersTempList = null;

            string orderByParam = mState.OrderBy.Trim();

            if (id >= 0)
            {
                // There was a singer selected
                singersTempList = totalSingers.Where(x=>x.Id == id);
            }
            else
            {
                // No singer selected
                if (orderByParam == "")
                {
                    int sing_id = singer.Id;
                    singersTempList = totalSingers.Where(x => (x.Id == sing_id) );
                }
                else if (orderByParam.Equals("SingNo", StringComparison.OrdinalIgnoreCase))
                {
                    string sing_no = singer.SingNo.Trim();
                    int len = sing_no.Length;
                    singersTempList = totalSingers.Where(x=>x.SingNo.Trim().Substring(0, len) == sing_no);
                }
                else if (orderByParam.Equals("SingNa", StringComparison.OrdinalIgnoreCase))
                {
                    string sing_na = singer.SingNa.Trim();
                    int len = sing_na.Length;
                    singersTempList = totalSingers.Where(x => x.SingNa.Trim().Substring(0, len) == sing_na);
                }
                else
                {
                    // not inside range of roder by then return empty lsit
                    return new List<Singer>(); 
                }
            }

            int totalRecords = totalSingers.Count();  // the whole singer table

            bool isFound = true;
            singerWithIndex = singersTempList.FirstOrDefault(); // the first one found
            if (singerWithIndex == null)
            {
                if (totalRecords == 0)
                {
                    // Singer Table is empty
                    UpdateStateOfRequest(mState, singerWithIndex, mState.CurrentPageNo, pageSize, 0, 0, true);
                    // return empty list
                    return new List<Singer>();
                }
                else
                {
                    // go to last page
                    singerWithIndex = totalSingers.LastOrDefault();
                }
            }
        
            singer.CopyFrom(singerWithIndex);   // return to calling function

            // find the row number of singerWithIndex
            int tempCount = 0;
            foreach (var singerVar in totalSingers)
            {
                ++tempCount;    // first row number is 1
                if (singerVar.Id == singerWithIndex.Id)
                {
                    break;
                }
            }
            int pageNo =  tempCount / pageSize;
            if ( (pageNo * pageSize) != tempCount)
            {
                pageNo++;
            }

            int recordNo = (pageNo - 1) * pageSize;

            singers = totalSingers.Skip(recordNo).Take(pageSize).ToList();

            int totalPages = totalRecords / pageSize;
            if ((totalPages * pageSize) != totalRecords)
            {
                totalPages++;
            }

            if (isFound)
            {
                // found
                mState.OrgId = singer.Id; // chnaged OrgId to the singer id found
            }
            else
            {
                // not found, then it is last page and last record
                mState.OrgId = 0;   // no singer is selected
            }
            UpdateStateOfRequest(mState, singers.FirstOrDefault(), pageNo, pageSize, totalRecords, totalPages, true);

            return singers;
        }

        /// <summary>
        /// Finds the one singer by singer no.
        /// </summary>
        /// <returns>The one singer by singer no.</returns>
        /// <param name="sing_no">Singer no.</param>
        public async Task<Singer> FindOneSingerBySingNo(string sing_no)
        {
            Singer singer = await _context.Singer.Include(x=>x.Singarea)
                            .Where(x=>x.SingNo == sing_no).SingleOrDefaultAsync();

            return singer;
        }

        /// <summary>
        /// Finds the one singer by identifier.
        /// </summary>
        /// <returns>The one singer by identifier (Singer.Id).</returns>
        /// <param name="id">the id of the singer.</param>
        public async Task<Singer> FindOneSingerById(int id)
        {
            Singer singer = await _context.Singer.Include(x=>x.Singarea)
                            .Where(x=>x.Id == id).SingleOrDefaultAsync();

            return singer;
        }

        /// <summary>
        /// Adds the one singer to table.
        /// </summary>
        /// <returns>Return the error code.</returns>
        /// <param name="singer">Singer.</param>
        public async Task<int> AddOneSingerToTable(Singer singer)
        {
            int result = ErrorCodeModel.ErrorBecauseBugs;
            if (singer == null)
            {
                // the data for updating is empty
                result = ErrorCodeModel.SingerIsNull;
                return result;
            }
            if (string.IsNullOrEmpty(singer.SingNo))
            {
                // the singer no that input by user is empty
                result = ErrorCodeModel.SingerNoIsEmpty;
                return result;
            }
            Singer oldSinger = await FindOneSingerBySingNo(singer.SingNo);
            if (oldSinger != null)
            {
                // singer no is duplicate
                result = ErrorCodeModel.SingerNoDuplicate;
                return result;
            }

            // verifying the validation for singer data
            int validCode = await VerifySinger(singer);
            if (validCode != ErrorCodeModel.Succeeded)
            {
                // data is invalid
                result = validCode;
                return result;
            }
            using (var dbTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    _context.Add(singer);
                    await _context.SaveChangesAsync();
                    dbTransaction.Commit();
                    result = ErrorCodeModel.Succeeded;
                }
                catch (DbUpdateException ex)
                {
                    string errorMsg = ex.ToString();
                    Console.WriteLine("Failed to add one singer: \n" + errorMsg);
                    dbTransaction.Rollback();
                    result = ErrorCodeModel.DatabaseError;
                }
            }

            return result;
        }

        /// <summary>
        /// Updates the one singer by identifier.
        /// </summary>
        /// <returns>Return the error code</returns>
        /// <param name="id">Identifier.</param>
        /// <param name="singer">Singer.</param>
        public async Task<int> UpdateOneSingerById(int id, Singer singer)
        {
            int result = ErrorCodeModel.ErrorBecauseBugs;
            if (id == 0)
            {
                // its a bug, id of singer cannot be 0
                result = ErrorCodeModel.ErrorBecauseBugs;
                return result;
            }
            if (singer == null)
            {
                // the data for updating is empty
                result = ErrorCodeModel.SingerIsNull;
                return result;
            }
            if (string.IsNullOrEmpty(singer.SingNo))
            {
                // the singer no that input by user is empty
                result = ErrorCodeModel.SingerNoIsEmpty;
                return result;
            }
            Singer newSinger = await FindOneSingerBySingNo(singer.SingNo);
            if (newSinger != null)
            {
                if (newSinger.Id != id)
                {
                    // singer no is duplicate
                    result = ErrorCodeModel.SingerNoDuplicate;
                    return result;
                }
            }

            Singer orgSinger = await FindOneSingerById(id);
            if (orgSinger == null)
            {
                // the original singer does not exist any more
                result = ErrorCodeModel.OriginalSingerNotExist;
                return result;
            }
            else
            {
                orgSinger.CopyColumnsFrom(singer);

                // verifying the validation for Singer data
                int validCode = await VerifySinger(orgSinger);
                if (validCode != ErrorCodeModel.Succeeded)
                {
                    // data is invalid
                    result = validCode;
                    return result;
                }
                 
                // check if entry state changed
                if ( (_context.Entry(orgSinger).State) == EntityState.Modified)
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
                            Console.WriteLine("Failed to update singer table: \n" + msg);
                            dbTransaction.Rollback();
                            result = ErrorCodeModel.DatabaseError;
                        }
                    }
                }
                else
                {
                    result = ErrorCodeModel.SingerNotChanged; // no changed
                }
            }

            return result;
        }

        /// <summary>
        /// Deletes the one singer by singer no.
        /// </summary>
        /// <returns>Return the error code.</returns>
        /// <param name="sing_no">Singer no.</param>
        public async Task<int> DeleteOneSingerBySingNo(string sing_no)
        {
            int result = ErrorCodeModel.ErrorBecauseBugs;
            if (string.IsNullOrEmpty(sing_no))
            {
                // its a bug, the original singer no is empty
                result = ErrorCodeModel.OriginalSingerNoIsEmpty;
                return result;
            }

            Singer orgSinger = await FindOneSingerBySingNo(sing_no);
            if (orgSinger == null)
            {
                // the original singer does not exist any more
                result = ErrorCodeModel.OriginalSingerNotExist;
            }
            else
            {
                using (var dbTransaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        _context.Singer.Remove(orgSinger);
                        await _context.SaveChangesAsync();
                        dbTransaction.Commit();
                        result = ErrorCodeModel.Succeeded; // succeeded to update
                    }
                    catch (DbUpdateException ex)
                    {
                        string msg = ex.ToString();
                        Console.WriteLine("Failed to delete one singer. Please see log file.\n" + msg);
                        dbTransaction.Rollback();
                        result = ErrorCodeModel.DatabaseError;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Deletes the one singer by identifier.
        /// </summary>
        /// <returns>Return the error code.</returns>
        /// <param name="id">Identifier.</param>
        public async Task<int> DeleteOneSingerById(int id)
        {
            int result = ErrorCodeModel.ErrorBecauseBugs;
            if (id == 0)
            {
                // its a bug, the id of singer cannot be 0
                result = ErrorCodeModel.ErrorBecauseBugs;
                return result;
            }

            Singer orgSinger = await FindOneSingerById(id);
            if (orgSinger == null)
            {
                // the original singer does not exist any more
                result = ErrorCodeModel.OriginalSingerNotExist;
            }
            else
            {
                using (var dbTransaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        _context.Singer.Remove(orgSinger);
                        await _context.SaveChangesAsync();
                        dbTransaction.Commit();
                        result = ErrorCodeModel.Succeeded; // succeeded to update
                    }
                    catch (DbUpdateException ex)
                    {
                        string msg = ex.ToString();
                        Console.WriteLine("Failed to delete one singer. Please see log file.\n" + msg);
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
        // ~SingerManager() {
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
