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
        // end of private methods


        // public methods
        public async Task<List<Playerscore>> GetAllPlayerscores(StateOfRequest mState)
        {
            if (mState == null)
            {
                return new List<Playerscore>();    // return empty list
            }

            mState.CurrentPageNo = -100;  // represnt to get all languages
            List<Playerscore> totalPlayerscores = await GetOnePageOfPlayerscoresDictionary(mState);

            return totalPlayerscores;
        }

        /// <summary>
        /// Gets the dictionary of playerscores.
        /// </summary>
        /// <returns>The dictionary of playerscores.</returns>
        /// <param name="mState">Playerscore state.</param>
        public async Task<SortedDictionary<int, Playerscore>> GetDictionaryOfPlayerscores(StateOfRequest mState)
        {
            if (mState == null)
            {
                return new SortedDictionary<int, Playerscore>();
            }

            List<Playerscore> totalPlayerscores = await _context.Playerscore.AsNoTracking().ToListAsync();

            Dictionary<int, Playerscore> playerscoresDictionary = null;

            // No playerscore selected
            if (mState.OrderBy == "")
            {
                playerscoresDictionary = totalPlayerscores
                            .Select((m, index) => new { rowNumber = index + 1, m })
                            .ToDictionary(m => m.rowNumber, m => m.m);
            }
            else if (mState.OrderBy == "PlayerName")
            {
                playerscoresDictionary = totalPlayerscores.OrderBy(x => x.PlayerName)
                            .Select((m, index) => new { rowNumber = index + 1, m })
                            .ToDictionary(m => m.rowNumber, m => m.m);
            }
            else if (mState.OrderBy == "Score")
            {
                playerscoresDictionary = totalPlayerscores.OrderBy(x => x.Score).ThenBy(x => x.PlayerName)
                            .Select((m, index) => new { rowNumber = index + 1, m })
                            .ToDictionary(m => m.rowNumber, m => m.m);
            }
            else
            {
                // not inside range of roder by
                playerscoresDictionary = new Dictionary<int, Playerscore>();    // empty lsit
            }

            return new SortedDictionary<int, Playerscore>(playerscoresDictionary);
        }

        /// <summary>
        /// Gets the select list from a SortedDictionary of playerscores.
        /// </summary>
        /// <returns>The select list of playerscores.</returns>
        /// <param name="mState">Playerscore state.</param>
        public async Task<List<SelectListItem>> GetSelectListOfPlayerscores(StateOfRequest mState)
        {
            List<SelectListItem> selectList = new List<SelectListItem>();
            List<Playerscore> playerscores = await GetAllPlayerscores(mState);
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
        /// Gets the total page of playerscore table.
        /// </summary>
        /// <returns>The total page of Playerscore table.</returns>
        public async Task<int[]> GetTotalRecordsAndPages(int pageSize)    // by condition
        {
            int[] result = new int[2] { 0, 0 };

            if (pageSize <= 0)
            {
                Console.WriteLine("The value of pageSize cannot be less than 0.");
                return result;
            }
            // have to define queryCondition
            // queryCondition has not been used for now

            int count = await _context.Playerscore.CountAsync();
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
        /// Gets the one page of playerscores dictionary.
        /// </summary>
        /// <returns>The one page of playerscores dictionary.</returns>
        /// <param name="mState">Playerscore state.</param>
        public async Task<List<Playerscore>> GetOnePageOfPlayerscoresDictionary(StateOfRequest mState)
        {
            if (mState == null)
            {
                return new List<Playerscore>();
            }
            if (string.IsNullOrEmpty(mState.OrderBy))
            {
                mState.OrderBy = "PlayerName";
            }

            SortedDictionary<int, Playerscore> playerscoresDictionary = await GetDictionaryOfPlayerscores(mState);

            int pageNo = mState.CurrentPageNo;
            int pageSize = mState.PageSize;
            int totalRecords = playerscoresDictionary.Count;
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

            List<Playerscore> playerscores;
            if (getAll)
            {
                playerscores = playerscoresDictionary.Select(m => m.Value).ToList();
            }
            else 
            {
                playerscores = playerscoresDictionary.Skip(recordNo).Take(pageSize).Select(m => m.Value).ToList();
            }

            mState.CurrentPageNo = pageNo;
            mState.PageSize = pageSize;
            mState.TotalRecords = totalRecords;
            mState.TotalPages = totalPages;

            Playerscore firstPlayerscore = playerscores.FirstOrDefault();
            if (firstPlayerscore != null)
            {
                mState.FirstId = firstPlayerscore.Id;
            }
            else
            {
                mState.OrgId = 0;
                mState.OrgNo = "";
                mState.FirstId = 0;
            }

            return playerscores;
        }

        /// <summary>
        /// Finds the one page of playerscores for one playerscore.
        /// </summary>
        /// <returns>The one page of playerscores for one playerscore.</returns>
        /// <param name="mState">Playerscore state.</param>
        /// <param name="playerscore">Playerscore.</param>
        /// <param name="id">Identifier.</param>
        public async Task<List<Playerscore>> FindOnePageOfPlayerscoresForOnePlayerscore(StateOfRequest mState, Playerscore playerscore, int id)
        {
            if ((mState == null) || (playerscore == null))
            {
                return new List<Playerscore>();
            }
            if (string.IsNullOrEmpty(mState.OrderBy))
            {
                mState.OrderBy = "PlayerName";
            }

            int pageSize = mState.PageSize;

            List<Playerscore> playerscores = null;
            KeyValuePair<int, Playerscore> playerscoreWithIndex = new KeyValuePair<int, Playerscore>(-1, null);

            SortedDictionary<int, Playerscore> playerscoresDictionary = await GetDictionaryOfPlayerscores(mState);

            if (id >= 0)
            {
                // There was a selected playerscore
                playerscoreWithIndex = playerscoresDictionary.Where(x => x.Value.Id == id).SingleOrDefault();
            }
            else
            {
                // No playerscore selected
                if (mState.OrderBy == "")
                {
                    int player_id = playerscore.Id;
                    playerscoreWithIndex = playerscoresDictionary.Where(x => (x.Value.Id >= player_id)).FirstOrDefault();
                }
                else if (mState.OrderBy == "PlayerName")
                {
                    string playerName = playerscore.PlayerName;
                    playerscoreWithIndex = playerscoresDictionary.Where(x => (String.Compare(x.Value.PlayerName, playerName) >= 0)).FirstOrDefault();
                }
                else if (mState.OrderBy == "Score")
                {
                    int score = playerscore.Score;
                    playerscoreWithIndex = playerscoresDictionary.Where(x =>x.Value.Score >= score).FirstOrDefault();
                }
                else
                {
                    // not inside range of roder by then return empty lsit
                    return new List<Playerscore>();
                }
            }

            if (playerscoreWithIndex.Value == null)
            {
                if (playerscoresDictionary.Count == 0)
                {
                    // dictionary (playerscore Table) is empty
                    mState.OrgId = 0;
                    mState.OrgNo = "";
                    mState.FirstId = 0;
                    // return empty list
                    return new List<Playerscore>();
                }
                else
                {
                    // go to last page
                    playerscoreWithIndex = playerscoresDictionary.LastOrDefault();
                }
            }

            Playerscore playerscoreFound = playerscoreWithIndex.Value;
            playerscore.CopyFrom(playerscoreFound);

            int tempCount = playerscoreWithIndex.Key;
            int pageNo = tempCount / pageSize;
            if ((pageNo * pageSize) != tempCount)
            {
                pageNo++;
            }
            int recordNo = (pageNo - 1) * pageSize;
            playerscores = playerscoresDictionary.Skip(recordNo).Take(pageSize).Select(m => m.Value).ToList();

            int totalRecords = playerscoresDictionary.Count;
            int totalPages = totalRecords / pageSize;
            if ((totalPages * pageSize) != totalRecords)
            {
                totalPages++;
            }

            mState.CurrentPageNo = pageNo;
            mState.PageSize = pageSize;
            mState.TotalRecords = totalRecords;
            mState.TotalPages = totalPages;
            mState.OrgId = playerscore.Id;
            mState.OrgNo = playerscore.PlayerName;

            Playerscore firstPlayerscore = playerscores.FirstOrDefault();
            if (firstPlayerscore != null)
            {
                mState.FirstId = firstPlayerscore.Id;
            }
            else
            {
                mState.FirstId = 0;
            }

            return playerscores;
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

            try
            {
                _context.Add(playerscore);
                await _context.SaveChangesAsync();
                result = ErrorCodeModel.Succeeded;
            }
            catch (DbUpdateException ex)
            {
                string errorMsg = ex.ToString();
                Console.WriteLine("Failed to add one playerscore: \n" + errorMsg);
                result = ErrorCodeModel.DatabaseError;
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

            try
            {
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
                        await _context.SaveChangesAsync();
                        result = ErrorCodeModel.Succeeded; // succeeded to update
                    }
                    else
                    {
                        result = ErrorCodeModel.PlayerscoreNotChanged; // no changed
                    }
                }
            }
            catch (DbUpdateException ex)
            {
                string msg = ex.ToString();
                Console.WriteLine("Failed to update Playerscore table: \n" + msg);
                result = ErrorCodeModel.DatabaseError;
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
            try
            {
                Playerscore orgPlayerscore = await FindOnePlayerscoreById(id);
                if (orgPlayerscore == null)
                {
                    // the original playerscore does not exist any more
                    result = ErrorCodeModel.OriginalPlayerscoreNotExist;
                }
                else
                {
                    _context.Playerscore.Remove(orgPlayerscore);
                    await _context.SaveChangesAsync();
                    result = ErrorCodeModel.Succeeded; // succeeded to update
                }
            }
            catch (DbUpdateException ex)
            {
                string msg = ex.ToString();
                Console.WriteLine("Failed to delete one playerscore. Please see log file.\n" + msg);
                result = ErrorCodeModel.DatabaseError;
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
