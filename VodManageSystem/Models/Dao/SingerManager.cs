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
    public class SingerManager : IDisposable
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
        public SingerManager(KtvSystemDBContext context)
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
            if (singer.AreaId >= 0 )
            {
                Singarea area = await _context.Singarea.Where(x => x.Id == singer.AreaId).SingleOrDefaultAsync();
                if (area != null)
                {
                    result = 1; // found singarea
                }
            }

            return result;
        }

        // end of private methods


        // public methods
        public async Task<List<Singer>> GetAllSingers(StateOfRequest mState) {
            if (mState == null)
            {
                return new List<Singer>();  // return empty list
            }

            mState.CurrentPageNo = -100;   // present to get all singers
            List<Singer> totalSingers = await GetOnePageOfSingersDictionary(mState);

            return totalSingers;
        }

        public async Task<List<Singer>> GetOnePageOfSingers(StateOfRequest mState)
        {
            if (mState == null)
            {
                return new List<Singer>();
            }

            var singersList = _context.Singer.Where(x => x.Id == -1);
            if (mState.OrderBy == "SingNo")
            {
                singersList = _context.Singer.Include(x => x.Singarea)
                                      .OrderBy(x => x.SingNo);
            }
            else if (mState.OrderBy == "SingNa")
            {
                singersList = _context.Singer.Include(x => x.Singarea)
                                      .OrderBy(x => x.SingNa).ThenBy(x => x.SingNo);
            }
            else if (mState.OrderBy == "")
            {
                singersList = _context.Singer.Include(x => x.Singarea);
            }
            else
            {
                // invalid order by then return empty list
            }
            int pageNo = mState.CurrentPageNo;
            int pageSize = mState.PageSize;
            int[] returnNumbers = await GetTotalRecordsAndPages(pageSize);
            int totalRecords = returnNumbers[0];
            int totalPages = returnNumbers[1];
            bool getAll = false;
            if (pageNo == -1)
            {
                // get the last page
                pageNo = totalPages;
            }
            else if (pageNo == -100)
            {
                // get all songs
                getAll = true;
                pageNo = 1; // restore pageNo to 1
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
            List<Singer> singers;
            if (getAll)
            {
                singers = await singersList.AsNoTracking().ToListAsync();
            }
            else
            {
                singers = await singersList.Skip(recordNum).Take(pageSize).AsNoTracking().ToListAsync();
            }
            mState.CurrentPageNo = pageNo;
            mState.PageSize = pageSize;
            mState.TotalRecords = totalRecords;
            mState.TotalPages = totalPages;
            Singer firstSinger = singers.FirstOrDefault();
            if (firstSinger != null)
            {
                mState.FirstId = firstSinger.Id;
            }
            else
            {
                mState.OrgId = 0;
                mState.OrgNo = "";
                mState.FirstId = 0;
            }
            return singers;
        }

        /// <summary>
        /// Gets the dictionary of singers.
        /// </summary>
        /// <returns>The dictionary of singers.</returns>
        /// <param name="mState">Singer state.</param>
        public async Task<SortedDictionary<int, Singer>> GetDictionaryOfSingers(StateOfRequest mState)
        {
            if (mState == null)
            {
                return new SortedDictionary<int, Singer>();
            }

            List<Singer> totalSingers = await _context.Singer.Include(x => x.Singarea)
                                                .AsNoTracking().ToListAsync();

            Dictionary<int, Singer> singersDictionary = null;

            // No singer selected
            if (mState.OrderBy == "")
            {
                singersDictionary = totalSingers
                            .Select((m, index) => new { rowNumber = index + 1, m })
                            .ToDictionary(m => m.rowNumber, m => m.m);
            }
            else if (mState.OrderBy == "SingNo")
            {
                singersDictionary = totalSingers.OrderBy(x => x.SingNo)
                            .Select((m, index) => new { rowNumber = index + 1, m })
                            .ToDictionary(m => m.rowNumber, m => m.m);
            }
            else if (mState.OrderBy == "SingNa")
            {
                singersDictionary = totalSingers.OrderBy(x => x.SingNa).ThenBy(x => x.SingNo)
                            .Select((m, index) => new { rowNumber = index + 1, m })
                            .ToDictionary(m => m.rowNumber, m => m.m);
            }
            else
            {
                // not inside range of roder by
                singersDictionary = new Dictionary<int, Singer>();    // empty lsit
            }

            return new SortedDictionary<int, Singer>(singersDictionary);
        }

        /// <summary>
        /// Gets the select list from a SortedDictionary of singers.
        /// </summary>
        /// <returns>The select list of singers.</returns>
        /// <param name="mState">Singer state.</param>
        public async Task<List<SelectListItem>> GetSelectListOfSingers(StateOfRequest mState)
        {
            List<SelectListItem> selectList = new List<SelectListItem>();
            List<Singer> singers = await GetAllSingers(mState);
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
        /// Gets the total page of singer table.
        /// </summary>
        /// <returns>The total page of Singer table.</returns>
        public async Task<int[]> GetTotalRecordsAndPages(int pageSize)    // by condition
        {
            int[] result = new int[2] { 0, 0 };

            if (pageSize <= 0 )
            {
                Console.WriteLine("The value of pageSize cannot be less than 0.");
                return result;
            }
            // have to define queryCondition
            // queryCondition has not been used for now

            int count = await _context.Singer.CountAsync();
            int totalPages = count / pageSize;
            if ((totalPages * pageSize) != count)
            {
                totalPages++;
            }

            result[0] = count;
            result[1] = totalPages;

            return result;
        }

        /// <summary>
        /// Gets the one page of singers dictionary.
        /// </summary>
        /// <returns>The one page of singers dictionary.</returns>
        /// <param name="mState">Singer state.</param>
        public async Task<List<Singer>> GetOnePageOfSingersDictionary(StateOfRequest mState)
        {
            if (mState == null)
            {
                return new List<Singer>();
            }
            if (string.IsNullOrEmpty(mState.OrderBy))
            {
                // default is order by singer's No
                mState.OrderBy = "SingNo";
            }

            SortedDictionary<int, Singer> singersDictionary = await GetDictionaryOfSingers(mState);

            int pageNo = mState.CurrentPageNo;
            int pageSize = mState.PageSize;
            int totalRecords = singersDictionary.Count;
            int totalPages = totalRecords / pageSize;
            if ((totalPages * pageSize) < totalRecords)
            {
                totalPages++;
            }

            bool getAll = false;
            if (pageNo == -1)
            {
                // get the last page
                pageNo = totalPages;
            }
            else if (pageNo == -100)
            {
                // get all songs
                getAll = true;
                pageNo = 1; // restore pageNo to 1
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

            int recordNo = (pageNo - 1) * pageSize;

            List<Singer> singers;
            if (getAll)
            {
                singers = singersDictionary.Select(m => m.Value).ToList();
            }
            else
            {
                singers = singersDictionary.Skip(recordNo).Take(pageSize).Select(m => m.Value).ToList();
            }

            mState.CurrentPageNo = pageNo;
            mState.PageSize = pageSize;
            mState.TotalRecords = totalRecords;
            mState.TotalPages = totalPages;

            Singer firstSinger = singers.FirstOrDefault();
            if (firstSinger != null)
            {
                mState.FirstId = firstSinger.Id;
            }
            else
            {
                mState.OrgId = 0;
                mState.OrgNo = "";
                mState.FirstId = 0;
            }

            return singers;
        }

        /// <summary>
        /// Gets the one page of singers by Singarea No and singer sex.
        /// </summary>
        /// <returns>The one page of singers dictionary.</returns>
        /// <param name="mState">Singer state.</param>
        /// <param name="areaId">Singer area No.</param>
        /// <param name="sex">Singer sex.</param>
        public async Task<List<Singer>> GetOnePageOfSingersByAreaSex(StateOfRequest mState, int areaId, string sex)
        {
            if (mState == null)
            {
                return new List<Singer>();
            }

            var singersSubTotal = _context.Singer.Where(x => x.AreaId == -1);
            if (mState.OrderBy == "")
            {
                if (sex == "0")
                {
                    singersSubTotal = _context.Singer.Where(x => x.AreaId == areaId);
                }
                else
                {
                    singersSubTotal = _context.Singer.Where(x => (x.AreaId == areaId) && (x.Sex == sex));
                }
            }
            else if (mState.OrderBy == "SingNo")
            {
                if (sex == "0")
                {
                    singersSubTotal = _context.Singer.Where(x => x.AreaId == areaId)
                                                .OrderBy(x => x.SingNo);
                }
                else
                {
                    singersSubTotal = _context.Singer.Where(x => (x.AreaId == areaId) && (x.Sex == sex))
                                                .OrderBy(x => x.SingNo);
                }
            }
            else if (mState.OrderBy == "SingNa")
            {
                if (sex == "0")
                {
                    singersSubTotal = _context.Singer.Where(x => x.AreaId == areaId)
                                              .OrderBy(x => x.SingNa).ThenBy(x => x.SingNo);
                }
                else
                {
                    singersSubTotal = _context.Singer.Where(x => (x.AreaId == areaId) && (x.Sex == sex))
                                              .OrderBy(x => x.SingNa).ThenBy(x => x.SingNo);
                }
            }
            else
            {
                // empty list

            }

            int pageNo = mState.CurrentPageNo;
            int pageSize = mState.PageSize;
            int totalRecords = await singersSubTotal.CountAsync();
            int totalPages = totalRecords / pageSize;
            if (totalPages * pageSize != totalRecords)
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
            int recordNum = (pageNo - 1) * pageSize;

            List<Singer> singers = await singersSubTotal.Skip(recordNum).Take(pageSize).ToListAsync();

            mState.CurrentPageNo = pageNo;
            mState.PageSize = pageSize;
            mState.TotalRecords = totalRecords;
            mState.TotalPages = totalPages;

            Singer firstSinger = singers.FirstOrDefault();
            if (firstSinger != null)
            {
                mState.FirstId = firstSinger.Id;
            }
            else
            {
                mState.OrgId = 0;
                mState.OrgNo = "";
                mState.FirstId = 0;
            }

            return singers;
        }

        /// <summary>
        /// Gets the one page of singers by Singarea No and singer sex.
        /// </summary>
        /// <returns>The one page of singers dictionary.</returns>
        /// <param name="mState">Singer state.</param>
        /// <param name="areaId">Singer area No.</param>
        /// <param name="sex">Singer sex.</param>
        public async Task<List<Singer>> GetOnePageOfSingersByAreaSexByToList(StateOfRequest mState, int areaId, string sex)
        {
            if (mState == null)
            {
                return new List<Singer>();
            }
            if (string.IsNullOrEmpty(mState.OrderBy))
            {
                // default is order by singer's No
                mState.OrderBy = "SingNo";
            }

            List<Singer> singersSubTotal;
            if (sex == "0")
            {
                singersSubTotal = await _context.Singer.Where(x => x.AreaId == areaId).ToListAsync();
            }
            else
            {
                singersSubTotal = await _context.Singer.Where(x => (x.AreaId == areaId) && (x.Sex == sex)).ToListAsync();
            }

            if (mState.OrderBy=="SingNo")
            {
                singersSubTotal = singersSubTotal.OrderBy(x => x.SingNo).ToList();
            }
            else if (mState.OrderBy=="SingNa")
            {
                singersSubTotal = singersSubTotal.OrderBy(x => x.SingNa).ThenBy(x=>x.SingNo).ToList();
            } 
            else
            {
            }

            int pageNo = mState.CurrentPageNo;
            int pageSize = mState.PageSize;
            int totalRecords = singersSubTotal.Count;
            int totalPages = totalRecords / pageSize;
            if (totalPages * pageSize != totalRecords)
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
            int recordNum = (pageNo - 1) * pageSize;

            List<Singer> singers = singersSubTotal.Skip(recordNum).Take(pageSize).ToList();

            mState.CurrentPageNo = pageNo;
            mState.PageSize = pageSize;
            mState.TotalRecords = totalRecords;
            mState.TotalPages = totalPages;

            Singer firstSinger = singers.FirstOrDefault();
            if (firstSinger != null)
            {
                mState.FirstId = firstSinger.Id;
            }
            else
            {
                mState.OrgId = 0;
                mState.OrgNo = "";
                mState.FirstId = 0;
            }

            return singers;
        }


        /// <summary>
        /// Finds the one page of singers for one singer.
        /// </summary>
        /// <returns>The one page of singers for one singer.</returns>
        /// <param name="mState">Singer state.</param>
        /// <param name="singer">Singer.</param>
        /// <param name="id">Identifier.</param>
        public async Task<List<Singer>> FindOnePageOfSingersForOneSinger(StateOfRequest mState, Singer singer, int id)
        {
            if ( (mState == null) || (singer == null) )
            {
                return new List<Singer>();
            }

            int pageSize = mState.PageSize;

            List<Singer> singers = null;
            KeyValuePair<int,Singer> singerWithIndex = new KeyValuePair<int, Singer>(-1,null);

            SortedDictionary<int, Singer> singersDictionary = await GetDictionaryOfSingers(mState);

            if (id >= 0)
            {
                // There was a singer selected
                singerWithIndex = singersDictionary.Where(x=>x.Value.Id == id).SingleOrDefault();
            }
            else
            {
                // No singer selected
                if (mState.OrderBy == "")
                {
                    int sing_id = singer.Id;
                    singerWithIndex = singersDictionary.Where(x => (x.Value.Id >= sing_id) ).FirstOrDefault();
                }
                else if (mState.OrderBy == "SingNo")
                {
                    string sing_no = singer.SingNo;
                    singerWithIndex = singersDictionary
                        .Where(x=>(String.Compare(x.Value.SingNo,sing_no) >= 0)).FirstOrDefault();
                }
                else if (mState.OrderBy == "SingNa")
                {
                    string sing_na = singer.SingNa;
                    singerWithIndex = singersDictionary
                        .Where(x => String.Compare(x.Value.SingNa, sing_na) >= 0).FirstOrDefault();
                }
                else
                {
                    // not inside range of roder by then return empty lsit
                    return new List<Singer>(); 
                }
            }

            if (singerWithIndex.Value == null)
            {
                if (singersDictionary.Count == 0)
                {
                    // dictionary (Singer Table) is empty
                    mState.OrgId = 0;
                    mState.OrgNo = "";
                    mState.FirstId = 0;
                    // return empty list
                    return new List<Singer>();
                }
                else
                {
                    // go to last page
                    singerWithIndex = singersDictionary.LastOrDefault();
                }
            }
        
            Singer singerFound = singerWithIndex.Value;
            singer.CopyFrom(singerFound);   // return to calling function

            int tempCount = singerWithIndex.Key;
            int pageNo =  tempCount / pageSize;
            if ( (pageNo * pageSize) != tempCount)
            {
                pageNo++;
            }

            int recordNo = (pageNo - 1) * pageSize;
            singers = singersDictionary.Skip(recordNo).Take(pageSize).Select(m=>m.Value).ToList();

            int totalRecords = singersDictionary.Count;
            int totalPages = totalRecords / pageSize;
            if ((totalPages * pageSize) != totalRecords)
            {
                totalPages++;
            }

            mState.CurrentPageNo = pageNo;
            mState.PageSize = pageSize;
            mState.TotalRecords = totalRecords;
            mState.TotalPages = totalPages;
            mState.OrgId = singer.Id;
            mState.OrgNo = singer.SingNo;

            Singer firstSinger = singers.FirstOrDefault();
            if(firstSinger != null)
            {
                mState.FirstId = firstSinger.Id;
            }
            else
            {
                mState.FirstId = 0;
            }
               
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
                // singer_no is duplicate
                result = ErrorCodeModel.SingerNoDuplicate;
                return result;
            }

            try
            {
                // verifying the validation for singer data
                int validCode = await VerifySinger(singer);
                if (validCode != ErrorCodeModel.Succeeded)
                {
                    // data is invalid
                    result = validCode;
                    return result;
                }

                _context.Add(singer);
                await _context.SaveChangesAsync();
                result = ErrorCodeModel.Succeeded;
            }
            catch (DbUpdateException ex)
            {
                string errorMsg = ex.ToString();
                Console.WriteLine("Failed to add one singer: \n" + errorMsg);
                result = ErrorCodeModel.DatabaseError;    
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

            try
            {
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

                    // verifying the validation for Song data
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
                        await _context.SaveChangesAsync();
                        result = ErrorCodeModel.Succeeded; // succeeded to update
                    }
                    else
                    {
                        result = ErrorCodeModel.SingerNotChanged; // no changed
                    }
                }
            }
            catch (DbUpdateException ex)
            {
                string msg = ex.ToString();
                Console.WriteLine("Failed to update singer table: \n" + msg);
                result = ErrorCodeModel.DatabaseError;
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
            try
            {
                Singer orgSinger = await FindOneSingerBySingNo(sing_no);
                if (orgSinger == null)
                {
                    // the original singer does not exist any more
                    result = ErrorCodeModel.OriginalSingerNotExist;
                }
                else
                {
                    _context.Singer.Remove(orgSinger);
                    await _context.SaveChangesAsync();
                    result = ErrorCodeModel.Succeeded; // succeeded to update
                }
            }
            catch (DbUpdateException ex)
            {
                string msg = ex.ToString();
                Console.WriteLine("Failed to delete one singer. Please see log file.\n" + msg);
                result = ErrorCodeModel.DatabaseError;
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
            try
            {
                Singer orgSinger = await FindOneSingerById(id);
                if (orgSinger == null)
                {
                    // the original singer does not exist any more
                    result = ErrorCodeModel.OriginalSingerNotExist;
                }
                else
                {
                    _context.Singer.Remove(orgSinger);
                    await _context.SaveChangesAsync();
                    result = ErrorCodeModel.Succeeded; // succeeded to update
                }
            }
            catch (DbUpdateException ex)
            {
                string msg = ex.ToString();
                Console.WriteLine("Failed to delete one singer. Please see log file.\n" + msg);
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
