using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VodManageSystem.Models.DataModels;

namespace VodManageSystem.Models.Dao
{
    public class SingerManager : IDisposable
    {
        private readonly int pageSize = 18;
        private readonly KtvSystemDBContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:VodManageSystem.Models.Dao.SingerManager"/> class.
        /// </summary>
        /// <param name="context">Context.</param>
        public SingerManager(KtvSystemDBContext context)
        {
            _context = context;
        }

        // private methods

        //

        // public methods

        public async Task<SortedDictionary<int, Singer>> GetDictionaryOfSingers(SingerStateOfRequest singerState)
        {
            if (singerState == null)
            {
                return new SortedDictionary<int, Singer>();
            }

            List<Singer> totalSingers = await _context.Singer.AsNoTracking().ToListAsync();

            Dictionary<int, Singer> singersDictionary = null;

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

        public async Task<List<SelectListItem>> GetSelectListOfSingers(SingerStateOfRequest singerState)
        {
            List<SelectListItem> selectList = new List<SelectListItem>();
            SortedDictionary<int, Singer> singerDict = await GetDictionaryOfSingers(singerState);
            foreach (Singer singer in singerDict.Values)
            {
                selectList.Add(new SelectListItem
                {
                    Text = singer.SingNa,
                    Value = singer.SingNo
                });
            }
            return selectList;
        }

        //

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
