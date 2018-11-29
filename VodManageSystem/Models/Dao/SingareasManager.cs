using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VodManageSystem.Models.DataModels;

namespace VodManageSystem.Models.Dao
{
    public class SingareasManager : IDisposable
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
        public SingareasManager(KtvSystemDBContext context)
        {
            _context = context;
        }

        // private methods

        /// <summary>
        /// Gets the total page of singarea table.
        /// </summary>
        /// <returns>The total page of singarea table.</returns>
        private int[] GetTotalRecordsAndPages(int pageSize)    // by condition
        {
            int[] result = new int[2] { 0, 0 };

            if (pageSize <= 0)
            {
                Console.WriteLine("the value of pageSize cannot be less than 0.");
                return result;
            }
            // have to define queryCondition
            // queryCondition has not been used for now

            int count = _context.Singarea.Count();
            int totalPages = count / pageSize;
            if ((totalPages * pageSize) != count)
            {
                totalPages++;
            }

            result[0] = count;
            result[1] = totalPages;

            return result;
        }

        private void UpdateStateOfRequest(StateOfRequest mState, Singarea firstSingarea, int pageNo, int pageSize, int totalRecords, int totalPages, bool isFind = false)
        {
            mState.CurrentPageNo = pageNo;
            mState.PageSize = pageSize;
            mState.TotalRecords = totalRecords;
            mState.TotalPages = totalPages;
            if (firstSingarea != null)
            {
                mState.FirstId = firstSingarea.Id;
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

        private IQueryable<Singarea> GetAllSingareasIQueryable(StateOfRequest mState)
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

            IQueryable<Singarea> totalSingareas = _context.Singarea;

            string orderByParam = mState.OrderBy.Trim();

            IQueryable<Singarea> singareas;
            if (orderByParam == "")
            {
                singareas = totalSingareas;
            }
            else if (orderByParam.Equals("AreaNo", StringComparison.OrdinalIgnoreCase))
            {
                singareas = totalSingareas.OrderBy(x => x.AreaNo);
            }
            else if (orderByParam.Equals("AreaNa", StringComparison.OrdinalIgnoreCase))
            {
                singareas = totalSingareas.OrderBy(x => x.AreaNa);
            }
            else
            {
                // not inside range of roder by
                singareas = null;   // empty lsit
            }

            return singareas;
        }

        // end of private methods


        // public methods
        public List<Singarea> GetAllSingareas(StateOfRequest mState) {

            if (mState == null)
            {
                return new List<Singarea>();    // return empty list
            }
            int pageSize = mState.PageSize;
            if (pageSize <= 0)
            {
                Console.WriteLine("The value of pageSize cannot be less than 0.");
                return new List<Singarea>();
            }

            mState.CurrentPageNo = -100; // present to all Singareas
            List<Singarea> totalSingareas = GetOnePageOfSingareas(mState);

            return totalSingareas;
        }

        public List<Singarea> GetOnePageOfSingareas(StateOfRequest mState)
        {
            if (mState == null)
            {
                return new List<Singarea>();
            }
            int pageSize = mState.PageSize;
            if (pageSize <= 0)
            {
                Console.WriteLine("The value of pageSize cannot be less than 0.");
                return new List<Singarea>();
            }

            IQueryable<Singarea> totalSingareas = GetAllSingareasIQueryable(mState);
            if (totalSingareas == null)
            {
                return new List<Singarea>();
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
                // get all singareas
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

            List<Singarea> singareas = totalSingareas.Skip(recordNum).Take(pageSize).ToList();

            UpdateStateOfRequest(mState, singareas.FirstOrDefault(), pageNo, pageSize, totalRecords, totalPages);

            return singareas;
        }

        /// <summary>
        /// Gets the select list from a SortedDictionary of singareas.
        /// </summary>
        /// <returns>The select list of singareas.</returns>
        /// <param name="mState">Singarea state.</param>
        public List<SelectListItem> GetSelectListOfSingareas(StateOfRequest mState)
        {
            List<SelectListItem> selectList = new List<SelectListItem>();
            List<Singarea> singareas = GetAllSingareas(mState);
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
        /// Finds the one page of singareas for one singarea.
        /// </summary>
        /// <returns>The one page of singareas for one singarea.</returns>
        /// <param name="mState">Singarea state.</param>
        /// <param name="singarea">Singarea.</param>
        /// <param name="id">Identifier.</param>
        public List<Singarea> FindOnePageOfSingareasForOneSingarea(StateOfRequest mState, Singarea singarea, int id)
        {
            if ( (mState == null) || (singarea == null) )
            {
                return new List<Singarea>();
            }
            int pageSize = mState.PageSize;
            if (pageSize <= 0)
            {
                Console.WriteLine("The value of pageSize cannot be less than 0.");
                return new List<Singarea>();
            }

            IQueryable<Singarea> totalSingareas = GetAllSingareasIQueryable(mState);
            if (totalSingareas == null)
            {
                return new List<Singarea>();
            }

            List<Singarea> singareas = null;
            Singarea singareaWithIndex = null;
            IQueryable<Singarea> singareasTempList = null;

            string orderByParam = mState.OrderBy.Trim();
            if (id >= 0)
            {
                // There was a selected singarea
                singareasTempList = totalSingareas.Where(x => x.Id == id);
            }
            else
            {
                // No singarea selected
                if (orderByParam == "")
                {
                    int area_id = singarea.Id;
                    singareasTempList = totalSingareas.Where(x => (x.Id == area_id));
                }
                else if (orderByParam.Equals("AreaNo", StringComparison.OrdinalIgnoreCase))
                {
                    string area_no = singarea.AreaNo.Trim();
                    int len = area_no.Length;
                    singareasTempList = totalSingareas.Where(x => x.AreaNo.Trim().Substring(0, len) == area_no);
                }
                else if (orderByParam.Equals("AreaNa", StringComparison.OrdinalIgnoreCase))
                {
                    string area_na = singarea.AreaNa.Trim();
                    int len = area_na.Length;
                    singareasTempList = totalSingareas.Where(x => x.AreaNa.Trim().Substring(0, len) == area_na);
                }
                else
                {
                    // not inside range of roder by then return empty lsit
                    return new List<Singarea>(); 
                }
            }

            int totalRecords = totalSingareas.Count();  // the whole singarea table

            bool isFound = true;
            singareaWithIndex = singareasTempList.FirstOrDefault(); // the first one found
            if (singareaWithIndex == null)
            {
                isFound = false;    // singarea that was assigned is not found
                if (totalRecords == 0)
                {
                    // Singarea Table is empty
                    UpdateStateOfRequest(mState, singareaWithIndex, mState.CurrentPageNo, pageSize, 0, 0, true);
                    // return empty list
                    return new List<Singarea>();
                }
                else
                {
                    // go to last page
                    singareaWithIndex = totalSingareas.LastOrDefault();
                }
            }

            singarea.CopyFrom(singareaWithIndex);

            // find the row number of singareaWithIndex
            int tempCount = 0;
            foreach (var singareaVar in totalSingareas)
            {
                ++tempCount;    // first row number is 1
                if (singareaVar.Id == singareaWithIndex.Id)
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

            singareas = totalSingareas.Skip(recordNo).Take(pageSize).ToList();

            int totalPages = totalRecords / pageSize;
            if ((totalPages * pageSize) != totalRecords)
            {
                totalPages++;
            }

            if (isFound)
            {
                // found
                mState.OrgId = singarea.Id; // chnaged OrgId to the singarea id found
            }
            else
            {
                // not found, then it is last page and last record
                mState.OrgId = 0;   // no singarea is selected
            }
            UpdateStateOfRequest(mState, singareas.FirstOrDefault(), pageNo, pageSize, totalRecords, totalPages, true);

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
                // singarea no is duplicate
                result = ErrorCodeModel.SingareaNoDuplicate;
                return result;
            }

            using (var dbTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    _context.Add(singarea);
                    await _context.SaveChangesAsync();
                    dbTransaction.Commit();
                    result = ErrorCodeModel.Succeeded;
                }
                catch (DbUpdateException ex)
                {
                    string errorMsg = ex.ToString();
                    Console.WriteLine("Failed to add one singarea: \n" + errorMsg);
                    dbTransaction.Rollback();
                    result = ErrorCodeModel.DatabaseError;
                }
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
                            Console.WriteLine("Failed to update singarea table: \n" + msg);
                            dbTransaction.Rollback();
                            result = ErrorCodeModel.DatabaseError;
                        }
                    }
                }
                else
                {
                    result = ErrorCodeModel.SingareaNotChanged; // no changed
                }
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

            Singarea orgSingarea = await FindOneSingareaByAreaNo(area_no);
            if (orgSingarea == null)
            {
                // the original singarea does not exist any more
                result = ErrorCodeModel.OriginalSingareaNotExist;
            }
            else
            {
                using (var dbTransaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        _context.Singarea.Remove(orgSingarea);
                        await _context.SaveChangesAsync();
                        dbTransaction.Commit();
                        result = ErrorCodeModel.Succeeded; // succeeded to update
                    }
                    catch (DbUpdateException ex)
                    {
                        string msg = ex.ToString();
                        Console.WriteLine("Failed to delete one singarea. Please see log file.\n" + msg);
                        dbTransaction.Rollback();
                        result = ErrorCodeModel.DatabaseError;
                    }
                }
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

            Singarea orgSingarea = await FindOneSingareaById(id);
            if (orgSingarea == null)
            {
                // the original singarea does not exist any more
                result = ErrorCodeModel.OriginalSingareaNotExist;
            }
            else
            {
                using (var dbTransaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        _context.Singarea.Remove(orgSingarea);
                        await _context.SaveChangesAsync();
                        dbTransaction.Commit();
                        result = ErrorCodeModel.Succeeded; // succeeded to update
                    }
                    catch (DbUpdateException ex)
                    {
                        string msg = ex.ToString();
                        Console.WriteLine("Failed to delete one singarea. Please see log file.\n" + msg);
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
