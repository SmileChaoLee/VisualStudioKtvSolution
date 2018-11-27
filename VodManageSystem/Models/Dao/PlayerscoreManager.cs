using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VodManageSystem.Models.DataModels;

namespace VodManageSystem.Models.Dao
{
    public class PlayerscoreManager : IDisposable
    {
        // private properties
        private readonly KtvSystemDBContext _context;
        // end of private properties

        // public properties
        // end of public properties

        public PlayerscoreManager(KtvSystemDBContext context)
        {
            _context = context;
        }

        // private methods
        /// <summary>
        /// Gets the total page of playerscore table.
        /// </summary>
        /// <returns>The total page of Playerscore table.</returns>
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

            int count = _context.Playerscore.Count();
            int totalPages = count / pageSize;
            if ((totalPages * pageSize) != count)
            {
                totalPages++;
            }

            result[0] = count;
            result[1] = totalPages;

            return result;
        }

        private void UpdateStateOfRequest(StateOfRequest mState, Playerscore firstPlayerscore, int pageNo, int pageSize, int totalRecords, int totalPages, bool isFind = false)
        {
            mState.CurrentPageNo = pageNo;
            mState.PageSize = pageSize;
            mState.TotalRecords = totalRecords;
            mState.TotalPages = totalPages;
            if (firstPlayerscore != null)
            {
                mState.FirstId = firstPlayerscore.Id;
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

        private IQueryable<Playerscore> GetAllPlayerscoresIQueryable(StateOfRequest mState)
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

            IQueryable<Playerscore> totalPlayerscores = _context.Playerscore;

            return totalPlayerscores;
        }

        // end of private methods

        // public methods
        public List<Playerscore> GetAllPlayerscores(StateOfRequest mState)
        {
            if (mState == null)
            {
                return new List<Playerscore>();    // return empty list
            }
            int pageSize = mState.PageSize;
            if (pageSize <= 0)
            {
                Console.WriteLine("The value of pageSize cannot be less than 0.");
                return new List<Playerscore>();    // return empty list
            }

            mState.CurrentPageNo = -100;  // represnt to get all languages
            List<Playerscore> totalPlayerscores = GetOnePageOfPlayerscores(mState);

            return totalPlayerscores;
        }

        public List<Playerscore> GetOnePageOfPlayerscores(StateOfRequest mState)
        {
            if (mState == null)
            {
                return new List<Playerscore>();
            }
            int pageSize = mState.PageSize;
            if (pageSize <= 0)
            {
                Console.WriteLine("The value of pageSize cannot be less than 0.");
                return new List<Playerscore>();
            }

            IQueryable<Playerscore> totalPlayerscores = GetAllPlayerscoresIQueryable(mState);
            if (totalPlayerscores == null)
            {
                return new List<Playerscore>();
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
                // get all playerscores
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

            List<Playerscore> playerscores = totalPlayerscores.Skip(recordNum).Take(pageSize).ToList();

            UpdateStateOfRequest(mState, playerscores.FirstOrDefault(), pageNo, pageSize, totalRecords, totalPages);

            return playerscores;
        }

        /// <summary>
        /// Gets the select list from a SortedDictionary of playerscores.
        /// </summary>
        /// <returns>The select list of playerscores.</returns>
        /// <param name="mState">Playerscore state.</param>
        public List<SelectListItem> GetSelectListOfPlayerscores(StateOfRequest mState)
        {
            List<SelectListItem> selectList = new List<SelectListItem>();
            List<Playerscore> playerscores = GetAllPlayerscores(mState);
            foreach (Playerscore playerscore in playerscores)
            {
                selectList.Add(new SelectListItem
                {
                    Text = playerscore.PlayerName,
                    Value = Convert.ToString(playerscore.Score)
                });
            }
            return selectList;
        }

        /// <summary>
        /// Finds the one playerscore by playerscore no.
        /// </summary>
        /// <returns>The one playerscore by playerscore no.</returns>
        /// <param name="playerName">Playerscore no.</param>
        public async Task<Playerscore> FindOnePlayerscoreByPlayerName(string playerName)
        {
            Playerscore playerscore = await _context.Playerscore.Where(x => x.PlayerName == playerName).SingleOrDefaultAsync();

            return playerscore;
        }

        /// <summary>
        /// Finds the one playerscore by identifier.
        /// </summary>
        /// <returns>The one playerscore by identifier (Playerscore.Id).</returns>
        /// <param name="id">the id of the playerscore.</param>
        public async Task<Playerscore> FindOnePlayerscoreById(int id)
        {
            Playerscore playerscore = await _context.Playerscore
                            .Where(x => x.Id == id).SingleOrDefaultAsync();

            return playerscore;
        }

        /// <summary>
        /// Adds the one playerscore to table.
        /// </summary>
        /// <returns>Return the error code.</returns>
        /// <param name="playerscore">Playerscore.</param>
        public async Task<int> AddOnePlayerscoreToTable(Playerscore playerscore)
        {
            int result = ErrorCodeModel.ErrorBecauseBugs;
            if (playerscore == null)
            {
                // the data for updating is empty
                result = ErrorCodeModel.PlayerscoreIsNull;
                return result;
            }
            if (string.IsNullOrEmpty(playerscore.PlayerName))
            {
                // the playerscore no that input by user is empty
                result = ErrorCodeModel.PlayerNameIsEmpty;
                return result;
            }

            using (var dbTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    _context.Add(playerscore);
                    await _context.SaveChangesAsync();
                    dbTransaction.Commit();
                    result = ErrorCodeModel.Succeeded;
                }
                catch (DbUpdateException ex)
                {
                    string errorMsg = ex.ToString();
                    Console.WriteLine("Failed to add one playerscore: \n" + errorMsg);
                    dbTransaction.Rollback();
                    result = ErrorCodeModel.DatabaseError;
                }
            }

            return result;
        }

        /// <summary>
        /// Updates the one playerscore by identifier.
        /// </summary>
        /// <returns>Return the error code</returns>
        /// <param name="id">Identifier.</param>
        /// <param name="playerscore">Playerscore.</param>
        public async Task<int> UpdateOnePlayerscoreById(int id, Playerscore playerscore)
        {
            int result = ErrorCodeModel.ErrorBecauseBugs;
            if (id == 0)
            {
                // its a bug, id of playerscore cannot be 0
                result = ErrorCodeModel.ErrorBecauseBugs;
                return result;
            }
            if (playerscore == null)
            {
                // the data for updating is empty
                result = ErrorCodeModel.PlayerscoreIsNull;
                return result;
            }
            if (string.IsNullOrEmpty(playerscore.PlayerName))
            {
                // the playerscore name that input by user is empty
                result = ErrorCodeModel.PlayerNameIsEmpty;
                return result;
            }

            Playerscore orgPlayerscore = await FindOnePlayerscoreById(id);
            if (orgPlayerscore == null)
            {
                // the original playerscore does not exist any more
                result = ErrorCodeModel.OriginalPlayerscoreNotExist;
                return result;
            }
            else
            {
                orgPlayerscore.CopyFrom(playerscore);

                // check if entry state changed
                if ((_context.Entry(orgPlayerscore).State) == EntityState.Modified)
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
                            Console.WriteLine("Failed to update Playerscore table: \n" + msg);
                            dbTransaction.Rollback();
                            result = ErrorCodeModel.DatabaseError;
                        }
                    }
                }
                else
                {
                    result = ErrorCodeModel.PlayerscoreNotChanged; // no changed
                }
            }

            return result;
        }

        /// <summary>
        /// Deletes the one playerscore by identifier.
        /// </summary>
        /// <returns>Return the error code.</returns>
        /// <param name="id">Identifier.</param>
        public async Task<int> DeleteOnePlayerscoreById(int id)
        {
            int result = ErrorCodeModel.ErrorBecauseBugs;
            if (id == 0)
            {
                // its a bug, the id of playerscore cannot be 0
                result = ErrorCodeModel.ErrorBecauseBugs;
                return result;
            }

            Playerscore orgPlayerscore = await FindOnePlayerscoreById(id);
            if (orgPlayerscore == null)
            {
                // the original playerscore does not exist any more
                result = ErrorCodeModel.OriginalPlayerscoreNotExist;
            }
            else
            {
                using (var dbTransaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        _context.Playerscore.Remove(orgPlayerscore);
                        await _context.SaveChangesAsync();
                        dbTransaction.Commit();
                        result = ErrorCodeModel.Succeeded; // succeeded to update
                    }
                    catch (DbUpdateException ex)
                    {
                        string msg = ex.ToString();
                        Console.WriteLine("Failed to delete one playerscore. Please see log file.\n" + msg);
                        dbTransaction.Rollback();
                        result = ErrorCodeModel.DatabaseError;
                    }
                }
            }

            return result;
        }

        public async Task<List<Playerscore>> GetTop10ScoresList(int gameId)
        {
            List<Playerscore> top10List;
            if (gameId == 1) {
                top10List = await _context.Playerscore.Where(x => (x.GameId==0) || (x.GameId==1))
                                          .OrderByDescending(x => x.Score).Take(10).ToListAsync();
            } else {
                top10List = await _context.Playerscore.Where(x => x.GameId == gameId).OrderByDescending(x => x.Score).Take(10).ToListAsync();
            }

            return top10List;
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
        // ~PlayerscoreManager() {
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
