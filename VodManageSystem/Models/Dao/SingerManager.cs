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
        public async Task<List<Singer>> GetAllSingersAsync(SingerStateOfRequest singerState) {
            List<Singer> totalSingers;
            if (singerState.OrderBy == "SingNo")
            {
                totalSingers = await _context.Singer.Include(x => x.Singarea)
                                          .OrderBy(x=>x.SingNo)
                                          .AsNoTracking().ToListAsync();
            }
            else if (singerState.OrderBy == "SingNa")
            {
                totalSingers = await _context.Singer.Include(x => x.Singarea)
                                          .OrderBy(x => x.SingNo).ThenBy(x=>x.SingNo)
                                          .AsNoTracking().ToListAsync();
            }
            else
            {
                totalSingers = new List<Singer>();
            }
            return totalSingers;
        }

        /// <summary>
        /// Gets the dictionary of singers.
        /// </summary>
        /// <returns>The dictionary of singers.</returns>
        /// <param name="singerState">Singer state.</param>
        public async Task<SortedDictionary<int, Singer>> GetDictionaryOfSingers(SingerStateOfRequest singerState)
        {
            if (singerState == null)
            {
                return new SortedDictionary<int, Singer>();
            }

            List<Singer> totalSingers = await _context.Singer.Include(x => x.Singarea)
                                      .AsNoTracking().ToListAsync();

            Dictionary<int, Singer> singersDictionary = null;


            // OrderBy(x=>x.SingNo) must put the following (not above in the totalSingers)
            if (singerState.OrderBy == "SingNo")
            {
                singersDictionary = totalSingers.OrderBy(x => x.SingNo)
                            .Select((m, index) => new { rowNumber = index + 1, m })
                            .ToDictionary(m => m.rowNumber, m => m.m);
            }
            else if (singerState.OrderBy == "SingNa")
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
        /// <param name="singerState">Singer state.</param>
        public async Task<List<SelectListItem>> GetSelectListOfSingers(SingerStateOfRequest singerState)
        {
            List<SelectListItem> selectList = new List<SelectListItem>();
            SortedDictionary<int, Singer> singerDict = await GetDictionaryOfSingers(singerState);
            foreach (Singer sing in singerDict.Values)
            {
                selectList.Add(new SelectListItem
                {
                    Text = sing.SingNa,
                    Value = sing.SingNo
                });
            }
            return selectList;
        }

        /// <summary>
        /// Gets the total page of singer table.
        /// </summary>
        /// <returns>The total page of Singer table.</returns>
        public async Task<int> GetTotalPageOfSingerTable(int pageSize)    // by condition
        {
            if (pageSize <= 0 )
            {
                Console.WriteLine("The value of pageSize cannot be less than 0.");
                return 0;
            }
            // have to define queryCondition
            // queryCondition has not been used for now

            int count = await _context.Singer.CountAsync();
            int totalPages = count / pageSize;
            if ((totalPages * pageSize) != count)
            {
                totalPages++;
            }
            return totalPages;
        }

        /// <summary>
        /// Gets the one page of singers dictionary.
        /// </summary>
        /// <returns>The one page of singers dictionary.</returns>
        /// <param name="singerState">Singer state.</param>
        public async Task<List<Singer>> GetOnePageOfSingersDictionary(SingerStateOfRequest singerState)
        {
            if (singerState == null)
            {
                return new List<Singer>();
            }
            if (string.IsNullOrEmpty(singerState.OrderBy))
            {
                // default is order by singer's No
                singerState.OrderBy = "SingNo";
            }

            SortedDictionary<int, Singer> singersDictionary = await GetDictionaryOfSingers(singerState);

            int pageNo = singerState.CurrentPageNo;
            int pageSize = singerState.PageSize;
            int totalCount = singersDictionary.Count;
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
            List<Singer> singers = singersDictionary.Skip(recordNo).Take(pageSize).Select(m => m.Value).ToList();

            singerState.CurrentPageNo = pageNo;
            Singer firstSinger = singers.FirstOrDefault();
            if (firstSinger != null)
            {
                singerState.FirstId = firstSinger.Id;
            }
            else
            {
                singerState.OrgId = 0;
                singerState.OrgSingNo = "";
                singerState.FirstId = 0;
            }

            return singers;
        }

        public async Task<List<Singer>> GetOnePageOfSingers(SingerStateOfRequest singerState)
        {
            if (singerState == null)
            {
                return new List<Singer>();
            }
            if (string.IsNullOrEmpty(singerState.OrderBy))
            {
                // default is order by singer's No
                singerState.OrderBy = "SingNo";
            }

            int pageNo = singerState.CurrentPageNo;
            int pageSize = singerState.PageSize;
            int totalPages = await GetTotalPageOfSingerTable(pageSize);
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

            List<Singer> singers;

            if (singerState.OrderBy == "SingNo")
            {
                singers = await _context.Singer.Include(x => x.Singarea)
                                        .OrderBy(x => x.SingNo)
                                        .Skip(recordNum).Take(pageSize)
                                        .AsNoTracking().ToListAsync();
            }
            else if (singerState.OrderBy == "SingNa")
            {
                singers = await _context.Singer.Include(x => x.Singarea)
                                        .OrderBy(x => x.SingNa).ThenBy(x => x.SingNo)
                                        .Skip(recordNum).Take(pageSize)
                                        .AsNoTracking().ToListAsync();
            }
            else
            {
                singers = await _context.Singer.Include(x => x.Singarea)
                                        .Skip(recordNum).Take(pageSize)
                                        .AsNoTracking().ToListAsync();
            }

            singerState.CurrentPageNo = pageNo;
            Singer firstSinger = singers.FirstOrDefault();
            if (firstSinger != null)
            {
                singerState.FirstId = firstSinger.Id;
            }
            else
            {
                singerState.OrgId = 0;
                singerState.OrgSingNo = "";
                singerState.FirstId = 0;
            }

            return singers;
        }

        /// <summary>
        /// Gets the one page of singers by Singarea No and singer sex.
        /// </summary>
        /// <returns>The one page of singers dictionary.</returns>
        /// <param name="singerState">Singer state.</param>
        /// <param name="areaId">Singer area No.</param>
        /// <param name="sex">Singer sex.</param>
        public async Task<List<Singer>> GetOnePageOfSingersByAreaSex(SingerStateOfRequest singerState, int areaId, string sex)
        {
            if (singerState == null)
            {
                return new List<Singer>();
            }
            if (string.IsNullOrEmpty(singerState.OrderBy))
            {
                // default is order by singer's No
                singerState.OrderBy = "SingNo";
            }

            var singersSubTotal = _context.Singer.Where(x=>x.AreaId == -1);
            if (singerState.OrderBy=="SingNo")
            {
                if (sex == "0")
                {
                    singersSubTotal = _context.Singer.Where(x =>x.AreaId == areaId)
                                                .OrderBy(x=>x.SingNo);
                }
                else
                {
                    singersSubTotal = _context.Singer.Where(x => (x.AreaId == areaId) && (x.Sex==sex))
                                                .OrderBy(x => x.SingNo);
                }
            }
            else if (singerState.OrderBy=="SingNa")
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
                if (sex == "0")
                {
                    singersSubTotal = _context.Singer.Where(x => x.AreaId == areaId);
                }
                else
                {
                    singersSubTotal = _context.Singer.Where(x => (x.AreaId == areaId) && (x.Sex==sex));
                }
            }

            int pageNo = singerState.CurrentPageNo;
            int pageSize = singerState.PageSize;
            int reccordCount = await singersSubTotal.CountAsync();
            int totalPages = reccordCount / pageSize;
            if (totalPages * pageSize != reccordCount)
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

            singerState.CurrentPageNo = pageNo;
            Singer firstSinger = singers.FirstOrDefault();
            if (firstSinger != null)
            {
                singerState.FirstId = firstSinger.Id;
            }
            else
            {
                singerState.OrgId = 0;
                singerState.OrgSingNo = "";
                singerState.FirstId = 0;
            }

            return singers;
        }


        /// <summary>
        /// Finds the one page of singers for one singer.
        /// </summary>
        /// <returns>The one page of singers for one singer.</returns>
        /// <param name="singerState">Singer state.</param>
        /// <param name="singer">Singer.</param>
        /// <param name="id">Identifier.</param>
        public async Task<List<Singer>> FindOnePageOfSingersForOneSinger(SingerStateOfRequest singerState, Singer singer, int id)
        {
            if ( (singerState == null) || (singer == null) )
            {
                return new List<Singer>();
            }
            if (string.IsNullOrEmpty(singerState.OrderBy))
            {
                // default is order by singer's No
                singerState.OrderBy = "SingNo";
            }

            int pageSize = singerState.PageSize;

            List<Singer> singers = null;
            KeyValuePair<int,Singer> singerWithIndex = new KeyValuePair<int, Singer>(-1,null);

            SortedDictionary<int, Singer> singersDictionary = await GetDictionaryOfSingers(singerState);

            if (id > 0)
            {
                // There was a selected singer
                singerWithIndex = singersDictionary.Where(x=>x.Value.Id == id).SingleOrDefault();
            }
            else
            {
                // No selected anguage
                if (singerState.OrderBy == "SingNo")
                {
                    string sing_no = singer.SingNo;
                    singerWithIndex = singersDictionary
                        .Where(x=>(String.Compare(x.Value.SingNo,sing_no, false) >= 0)).FirstOrDefault();
                }
                else if (singerState.OrderBy == "SingNa")
                {
                    string sing_na = singer.SingNa;
                    singerWithIndex = singersDictionary
                        .Where(x => String.Compare(x.Value.SingNa, sing_na, false) >= 0).FirstOrDefault();
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
                    singerState.OrgId = 0;
                    singerState.OrgSingNo = "";
                    singerState.FirstId = 0;
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

            singerState.CurrentPageNo = pageNo;
            singerState.OrgId = singer.Id;
            singerState.OrgSingNo = singer.SingNo;

            Singer firstSinger = singers.FirstOrDefault();
            if(firstSinger != null)
            {
                singerState.FirstId = firstSinger.Id;
            }
            else
            {
                singerState.FirstId = 0;
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
