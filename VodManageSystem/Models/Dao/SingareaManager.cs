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
        public static readonly int pageSize = 15;
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

        /// <summary>
        /// Gets the dictionary of Singareas.
        /// </summary>
        /// <returns>The dictionary of Singareas.</returns>
        /// <param name="singareaState">Singarea state.</param>
        public async Task<SortedDictionary<int, Singarea>> GetDictionaryOfSingareas(SingareaStateOfRequest singareaState)
        {
            if (singareaState == null)
            {
                return new SortedDictionary<int, Singarea>();
            }

            List<Singarea> totalSingareas = await _context.Singarea.AsNoTracking().ToListAsync();

            Dictionary<int, Singarea> singareasDictionary = null;

            if (singareaState.OrderBy == "AreaNo")
            {
                singareasDictionary = totalSingareas.OrderBy(x => x.AreaNo)
                            .Select((m, index) => new { rowNumber = index + 1, m })
                            .ToDictionary(m => m.rowNumber, m => m.m);
            }
            else if (singareaState.OrderBy == "AreaNa")
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
        /// Gets the select list from a SortedDictionary of Singareas.
        /// </summary>
        /// <returns>The select list of Singareas.</returns>
        /// <param name="singareaState">Singarea state.</param>
        public async Task<List<SelectListItem>> GetSelectListOfSingareas(SingareaStateOfRequest singareaState)
        {
            List<SelectListItem> selectList = new List<SelectListItem>();
            SortedDictionary<int, Singarea> singareaDict = await GetDictionaryOfSingareas(singareaState);
            foreach (Singarea area in singareaDict.Values)
            {
                selectList.Add(new SelectListItem
                {
                    Text = area.AreaNa,
                    Value = area.AreaNo
                });
            }
            return selectList;
        }

        /// <summary>
        /// Gets the total page of Singarea table.
        /// </summary>
        /// <returns>The total page of Singarea table.</returns>
        public async Task<int> GetTotalPageOfSingareaTable()    // by condition
        {
            // have to define queryCondition
            // queryCondition has not been used for now

            int count = await _context.Singarea.CountAsync();
            int totalPages = count / pageSize;
            if ((totalPages * pageSize) != count)
            {
                totalPages++;
            }
            return totalPages;
        }

        /// <summary>
        /// Gets the one page of Singareas dictionary.
        /// </summary>
        /// <returns>The one page of Singareas dictionary.</returns>
        /// <param name="singareaState">Singarea state.</param>
        public async Task<List<Singarea>> GetOnePageOfSingareasDictionary(SingareaStateOfRequest singareaState)
        {
            if (singareaState == null)
            {
                return new List<Singarea>();
            }
            if (string.IsNullOrEmpty(singareaState.OrderBy))
            {
                singareaState.OrderBy = "AreaNo";
            }

            int pageNo = singareaState.CurrentPageNo;
            if (pageNo < 1)
            {
                pageNo = 1;
            }

            SortedDictionary<int, Singarea> singareasDictionary = await GetDictionaryOfSingareas(singareaState);

            int totalCount = singareasDictionary.Count;
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
            List<Singarea> singareas = singareasDictionary.Skip(recordNo).Take(pageSize).Select(m => m.Value).ToList();

            singareaState.CurrentPageNo = pageNo;
            Singarea firstSingarea = singareas.FirstOrDefault();
            if (firstSingarea != null)
            {
                singareaState.OrgId = firstSingarea.Id;
                singareaState.OrgAreaNo = firstSingarea.AreaNo;
                singareaState.FirstAreaId = firstSingarea.Id;
            }
            else
            {
                singareaState.OrgId = 0;
                singareaState.OrgAreaNo = "";
                singareaState.FirstAreaId = 0;
            }

            return singareas;

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
