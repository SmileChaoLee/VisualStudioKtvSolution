using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VodManageSystem.Models.DataModels;

namespace VodManageSystem.Models.Dao
{
    public class SingareaManager : IDisposable
    {
        // private properties
        private readonly KtvSystemDBContext _context;
        // end of private properties

        // public properties
        // end of public properties

        /// <summary>
        /// Initializes a new instance of the <see cref="T:VodManageSystem.Models.Dao.SingareaManager"/> class.
        /// </summary>
        /// <param name="context">Context.</param>
        public SingareaManager(KtvSystemDBContext context)
        {
            _context = context;
        }

        // private methods

        // end of private methods


        // public methods
        public async Task<List<Singarea>> GetAllSingareas(StateOfRequest mState) {

            if (mState == null)
            {
                return new List<Singarea>();    // return empty list
            }

            mState.CurrentPageNo = -100; // present to all Singareas
            List<Singarea> totalSingareas = await GetOnePageOfSingareasDictionary(mState);

            return totalSingareas;
        }

        public async Task<List<Singarea>> GetOnePageOfSingareas(StateOfRequest mState)
        {
            if (mState == null)
            {
                return new List<Singarea>();
            }

            var singareasList = _context.Singarea.Where(x => x.Id == -1);
            if (mState.OrderBy == "AreaNo")
            {
                singareasList = _context.Singarea.OrderBy(x => x.AreaNo);
            }
            else if (mState.OrderBy == "AreaNa")
            {
                singareasList = _context.Singarea.OrderBy(x => x.AreaNa).ThenBy(x => x.AreaNo);
            }
            else if (mState.OrderBy == "")
            {
                singareasList = _context.Singarea;
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
            List<Singarea> singareas;
            if (getAll)
            {
                singareas = await singareasList.AsNoTracking().ToListAsync();
            }
            else
            {
                singareas = await singareasList.Skip(recordNum).Take(pageSize).AsNoTracking().ToListAsync();
            }
            mState.CurrentPageNo = pageNo;
            mState.PageSize = pageSize;
            mState.TotalRecords = totalRecords;
            mState.TotalPages = totalPages;
            Singarea firstSingarea = singareas.FirstOrDefault();
            if (firstSingarea != null)
            {
                mState.FirstId = firstSingarea.Id;
            }
            else
            {
                mState.OrgId = 0;
                mState.OrgNo = "";
                mState.FirstId = 0;
            }
            return singareas;
        }

        /// <summary>
        /// Gets the dictionary of singareas.
        /// </summary>
        /// <returns>The dictionary of singareas.</returns>
        /// <param name="mState">Singarea state.</param>
        public async Task<SortedDictionary<int, Singarea>> GetDictionaryOfSingareas(StateOfRequest mState)
        {
            if (mState == null)
            {
                return new SortedDictionary<int, Singarea>();
            }

            List<Singarea> totalSingareas = await _context.Singarea.AsNoTracking().ToListAsync();

            Dictionary<int, Singarea> singareasDictionary = null;

            // No singarea selected
            if (mState.OrderBy == "")
            {
                singareasDictionary = totalSingareas
                            .Select((m, index) => new { rowNumber = index + 1, m })
                            .ToDictionary(m => m.rowNumber, m => m.m);
            }
            else if (mState.OrderBy == "AreaNo")
            {
                singareasDictionary = totalSingareas.OrderBy(x => x.AreaNo)
                            .Select((m, index) => new { rowNumber = index + 1, m })
                            .ToDictionary(m => m.rowNumber, m => m.m);
            }
            else if (mState.OrderBy == "AreaNa")
            {
                singareasDictionary = totalSingareas.OrderBy(x => x.AreaNa).ThenBy(x => x.AreaNo)
                            .Select((m, index) => new { rowNumber = index + 1, m })
                            .ToDictionary(m => m.rowNumber, m => m.m);
            }
            else
            {
                // not inside range of roder by
                singareasDictionary = new Dictionary<int, Singarea>();    // empty lsit
            }

            return new SortedDictionary<int, Singarea>(singareasDictionary);
        }

        /// <summary>
        /// Gets the select list from a SortedDictionary of singareas.
        /// </summary>
        /// <returns>The select list of singareas.</returns>
        /// <param name="mState">Singarea state.</param>
        public async Task<List<SelectListItem>> GetSelectListOfSingareas(StateOfRequest mState)
        {
            List<SelectListItem> selectList = new List<SelectListItem>();
            List<Singarea> singareas = await GetAllSingareas(mState);
            foreach (Singarea area in singareas)
            {
                selectList.Add(new SelectListItem
                {
                    Text = area.AreaNa,
                    Value = Convert.ToString(area.Id)
                });
            }
            return selectList;
        }

        /// <summary>
        /// Gets the total page of singarea table.
        /// </summary>
        /// <returns>The total page of singarea table.</returns>
        public async Task<int[]> GetTotalRecordsAndPages(int pageSize)    // by condition
        {
            int[] result = new int[2] { 0, 0 };

            if (pageSize <= 0)
            {
                Console.WriteLine("the value of pageSize cannot be less than 0.");
                return result;
            }
            // have to define queryCondition
            // queryCondition has not been used for now

            int count = await _context.Singarea.CountAsync();
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
        /// Gets the one page of singareas dictionary.
        /// </summary>
        /// <returns>The one page of singareas dictionary.</returns>
        /// <param name="mState">Singarea state.</param>
        public async Task<List<Singarea>> GetOnePageOfSingareasDictionary(StateOfRequest mState)
        {
            if (mState == null)
            {
                return new List<Singarea>();
            }
            if (string.IsNullOrEmpty(mState.OrderBy))
            {
                mState.OrderBy = "AreaNo";
            }

            SortedDictionary<int, Singarea> singareasDictionary = await GetDictionaryOfSingareas(mState);

            int pageNo = mState.CurrentPageNo;
            int pageSize = mState.PageSize;
            int totalRecords = singareasDictionary.Count;
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

            List<Singarea> singareas;
            if (getAll)
            {
                singareas = singareasDictionary.Select(m => m.Value).ToList();
            }
            else
            {
                singareas = singareasDictionary.Skip(recordNo).Take(pageSize).Select(m => m.Value).ToList();
            }

            mState.CurrentPageNo = pageNo;
            mState.PageSize = pageSize;
            mState.TotalRecords = totalRecords;
            mState.TotalPages = totalPages;

            Singarea firstSingarea = singareas.FirstOrDefault();
            if (firstSingarea != null)
            {
                mState.FirstId = firstSingarea.Id;
            }
            else
            {
                mState.OrgId = 0;
                mState.OrgNo = "";
                mState.FirstId = 0;
            }

            return singareas;
        }

        /// <summary>
        /// Finds the one page of singareas for one singarea.
        /// </summary>
        /// <returns>The one page of singareas for one singarea.</returns>
        /// <param name="mState">Singarea state.</param>
        /// <param name="singarea">Singarea.</param>
        /// <param name="id">Identifier.</param>
        public async Task<List<Singarea>> FindOnePageOfSingareasForOneSingarea(StateOfRequest mState, Singarea singarea, int id)
        {
            if ( (mState == null) || (singarea == null) )
            {
                return new List<Singarea>();
            }

            int pageSize = mState.PageSize;

            List<Singarea> singareas = null;
            KeyValuePair<int,Singarea> singareaWithIndex = new KeyValuePair<int, Singarea>(-1,null);

            SortedDictionary<int, Singarea> singareasDictionary = await GetDictionaryOfSingareas(mState);

            if (id >= 0)
            {
                // There was a selected singarea
                singareaWithIndex = singareasDictionary.Where(x=>x.Value.Id == id).SingleOrDefault();
            }
            else
            {
                // No singarea selected
                if (mState.OrderBy == "")
                {
                    int area_id = singarea.Id;
                    singareaWithIndex = singareasDictionary.Where(x => (x.Value.Id >= area_id)).FirstOrDefault();
                }
                else if (mState.OrderBy == "AreaNo")
                {
                    string area_no = singarea.AreaNo;
                    singareaWithIndex = singareasDictionary.Where(x=>(String.Compare(x.Value.AreaNo,area_no)>=0)).FirstOrDefault();
                }
                else if (mState.OrderBy == "AreaNa")
                {
                    string area_na = singarea.AreaNa;
                    singareaWithIndex = singareasDictionary.Where(x=>(String.Compare(x.Value.AreaNa,area_na)>=0)).FirstOrDefault();
                }
                else
                {
                    // not inside range of roder by then return empty lsit
                    return new List<Singarea>(); 
                }
            }

            if (singareaWithIndex.Value == null)
            {
                if (singareasDictionary.Count == 0)
                {
                    // dictionary (Singarea Table) is empty
                    mState.OrgId = 0;
                    mState.OrgNo = "";
                    mState.FirstId = 0;
                    // return empty list
                    return new List<Singarea>();
                }
                else
                {
                    // go to last page
                    singareaWithIndex = singareasDictionary.LastOrDefault();
                }
            }
        
            Singarea singareaFound = singareaWithIndex.Value;
            singarea.CopyFrom(singareaFound);

            int tempCount = singareaWithIndex.Key;
            int pageNo =  tempCount / pageSize;
            if ( (pageNo * pageSize) != tempCount)
            {
                pageNo++;
            }
            int recordNo = (pageNo - 1) * pageSize;
            singareas = singareasDictionary.Skip(recordNo).Take(pageSize).Select(m=>m.Value).ToList();

            int totalRecords = singareasDictionary.Count;
            int totalPages = totalRecords / pageSize;
            if ((totalPages * pageSize) != totalRecords)
            {
                totalPages++;
            }

            mState.CurrentPageNo = pageNo;
            mState.PageSize = pageSize;
            mState.TotalRecords = totalRecords;
            mState.TotalPages = totalPages;
            mState.OrgId = singarea.Id;
            mState.OrgNo = singarea.AreaNo;

            Singarea firstSingarea = singareas.FirstOrDefault();
            if(firstSingarea != null)
            {
                mState.FirstId = firstSingarea.Id;
            }
            else
            {
                mState.FirstId = 0;
            }
               
            return singareas;
        }

        /// <summary>
        /// Finds the one singarea by singarea no.
        /// </summary>
        /// <returns>The one singarea by singarea no.</returns>
        /// <param name="area_no">Singarea no.</param>
        public async Task<Singarea> FindOneSingareaByAreaNo(string area_no)
        {
            Singarea singarea = await _context.Singarea.Where(x=>x.AreaNo == area_no).SingleOrDefaultAsync();

            return singarea;
        }

        /// <summary>
        /// Finds the one singarea by identifier.
        /// </summary>
        /// <returns>The one singarea by identifier (Singarea.Id).</returns>
        /// <param name="id">the id of the singarea.</param>
        public async Task<Singarea> FindOneSingareaById(int id)
        {
            Singarea singarea = await _context.Singarea.Where(x=>x.Id == id).SingleOrDefaultAsync();

            return singarea;
        }

        /// <summary>
        /// Adds the one singarea to table.
        /// </summary>
        /// <returns>Return the error code.</returns>
        /// <param name="singarea">Singarea.</param>
        public async Task<int> AddOneSingareaToTable(Singarea singarea)
        {
            int result = ErrorCodeModel.ErrorBecauseBugs;
            if (singarea == null)
            {
                // the data for updating is empty
                result = ErrorCodeModel.SingareaIsNull;
                return result;
            }
            if (string.IsNullOrEmpty(singarea.AreaNo))
            {
                // the singarea no that input by user is empty
                result = ErrorCodeModel.SingareaNoIsEmpty;
                return result;
            }
            Singarea oldSingarea = await FindOneSingareaByAreaNo(singarea.AreaNo);
            if (oldSingarea != null)
            {
                // singarea_no is duplicate
                result = ErrorCodeModel.SingareaNoDuplicate;
                return result;
            }

            try
            {
                _context.Add(singarea);
                await _context.SaveChangesAsync();
                result = ErrorCodeModel.Succeeded;
            }
            catch (DbUpdateException ex)
            {
                string errorMsg = ex.ToString();
                Console.WriteLine("Failed to add one singarea: \n" + errorMsg);
                result = ErrorCodeModel.DatabaseError;    
            }

            return result;
        }

        /// <summary>
        /// Updates the one singarea by identifier.
        /// </summary>
        /// <returns>Return the error code</returns>
        /// <param name="id">Identifier.</param>
        /// <param name="singarea">Singarea.</param>
        public async Task<int> UpdateOneSingareaById(int id, Singarea singarea)
        {
            int result = ErrorCodeModel.ErrorBecauseBugs;
            if (id == 0)
            {
                // its a bug, id of singarea cannot be 0
                result = ErrorCodeModel.ErrorBecauseBugs;
                return result;
            }
            if (singarea == null)
            {
                // the data for updating is empty
                result = ErrorCodeModel.SingareaIsNull;
                return result;
            }
            if (string.IsNullOrEmpty(singarea.AreaNo))
            {
                // the singarea no that input by user is empty
                result = ErrorCodeModel.SingareaNoIsEmpty;
                return result;
            }
            Singarea newSingarea = await FindOneSingareaByAreaNo(singarea.AreaNo);
            if (newSingarea != null)
            {
                if (newSingarea.Id != id)
                {
                    // singarea no is duplicate
                    result = ErrorCodeModel.SingareaNoDuplicate;
                    return result;
                }
            }

            try
            {
                Singarea orgSingarea = await FindOneSingareaById(id);
                if (orgSingarea == null)
                {
                    // the original singarea does not exist any more
                    result = ErrorCodeModel.OriginalSingareaNotExist;
                    return result;
                }
                else
                {
                    orgSingarea.CopyColumnsFrom(singarea);
                    
                    // check if entry state changed
                    if ( (_context.Entry(orgSingarea).State) == EntityState.Modified)
                    {
                        await _context.SaveChangesAsync();
                        result = ErrorCodeModel.Succeeded; // succeeded to update
                    }
                    else
                    {
                        result = ErrorCodeModel.SingareaNotChanged; // no changed
                    }
                }
            }
            catch (DbUpdateException ex)
            {
                string msg = ex.ToString();
                Console.WriteLine("Failed to update singarea table: \n" + msg);
                result = ErrorCodeModel.DatabaseError;
            }
            return result;
        }

        /// <summary>
        /// Deletes the one singarea by singarea no.
        /// </summary>
        /// <returns>Return the error code.</returns>
        /// <param name="area_no">Singarea no.</param>
        public async Task<int> DeleteOneSingareaByAreaNo(string area_no)
        {
            int result = ErrorCodeModel.ErrorBecauseBugs;
            if (string.IsNullOrEmpty(area_no))
            {
                // its a bug, the original singarea no is empty
                result = ErrorCodeModel.OriginalSingareaNoIsEmpty;
                return result;
            }
            try
            {
                Singarea orgSingarea = await FindOneSingareaByAreaNo(area_no);
                if (orgSingarea == null)
                {
                    // the original singarea does not exist any more
                    result = ErrorCodeModel.OriginalSingareaNotExist;
                }
                else
                {
                    _context.Singarea.Remove(orgSingarea);
                    await _context.SaveChangesAsync();
                    result = ErrorCodeModel.Succeeded; // succeeded to update
                }
            }
            catch (DbUpdateException ex)
            {
                string msg = ex.ToString();
                Console.WriteLine("Failed to delete one singarea. Please see log file.\n" + msg);
                result = ErrorCodeModel.DatabaseError;
            }
            return result;
        }

        /// <summary>
        /// Deletes the one singarea by identifier.
        /// </summary>
        /// <returns>Return the error code.</returns>
        /// <param name="id">Identifier.</param>
        public async Task<int> DeleteOneSingareaById(int id)
        {
            int result = ErrorCodeModel.ErrorBecauseBugs;
            if (id == 0)
            {
                // its a bug, the id of singarea cannot be 0
                result = ErrorCodeModel.ErrorBecauseBugs;
                return result;
            }
            try
            {
                Singarea orgSingarea = await FindOneSingareaById(id);
                if (orgSingarea == null)
                {
                    // the original singarea does not exist any more
                    result = ErrorCodeModel.OriginalSingareaNotExist;
                }
                else
                {
                    _context.Singarea.Remove(orgSingarea);
                    await _context.SaveChangesAsync();
                    result = ErrorCodeModel.Succeeded; // succeeded to update
                }
            }
            catch (DbUpdateException ex)
            {
                string msg = ex.ToString();
                Console.WriteLine("Failed to delete one singarea. Please see log file.\n" + msg);
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
        // ~SingareaManager() {
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
