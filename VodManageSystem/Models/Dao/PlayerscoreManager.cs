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
        public static readonly int pageSize = 15;
        // end of public properties

        public PlayerscoreManager(KtvSystemDBContext context)
        {
            _context = context;
        }

        // private methods
        // end of private methods


        // public methods

        /// <summary>
        /// Gets the dictionary of playerscores.
        /// </summary>
        /// <returns>The dictionary of playerscores.</returns>
        /// <param name="playerscoreState">Playerscore state.</param>
        public async Task<SortedDictionary<int, Playerscore>> GetDictionaryOfPlayerscores(PlayerscoreStateOfRequest playerscoreState)
        {
            if (playerscoreState == null)
            {
                return new SortedDictionary<int, Playerscore>();
            }

            List<Playerscore> totalPlayerscores = await _context.Playerscore.AsNoTracking().ToListAsync();

            Dictionary<int, Playerscore> playerscoresDictionary = null;

            if (playerscoreState.OrderBy == "PlayerName")
            {
                playerscoresDictionary = totalPlayerscores.OrderBy(x => x.PlayerName)
                            .Select((m, index) => new { rowNumber = index + 1, m })
                            .ToDictionary(m => m.rowNumber, m => m.m);
            }
            else if (playerscoreState.OrderBy == "Score")
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
        /// <param name="playerscoreState">Playerscore state.</param>
        public async Task<List<SelectListItem>> GetSelectListOfPlayerscores(PlayerscoreStateOfRequest playerscoreState)
        {
            List<SelectListItem> selectList = new List<SelectListItem>();
            SortedDictionary<int, Playerscore> playerscoreDict = await GetDictionaryOfPlayerscores(playerscoreState);
            foreach (Playerscore playerscore in playerscoreDict.Values)
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
        public async Task<int> GetTotalPageOfPlayerscoreTable()    // by condition
        {
            // have to define queryCondition
            // queryCondition has not been used for now

            int count = await _context.Playerscore.CountAsync();
            int totalPages = count / pageSize;
            if ((totalPages * pageSize) != count)
            {
                totalPages++;
            }
            return totalPages;
        }

        /// <summary>
        /// Gets the one page of playerscores dictionary.
        /// </summary>
        /// <returns>The one page of playerscores dictionary.</returns>
        /// <param name="playerscoreState">Playerscore state.</param>
        public async Task<List<Playerscore>> GetOnePageOfPlayerscoresDictionary(PlayerscoreStateOfRequest playerscoreState)
        {
            if (playerscoreState == null)
            {
                return new List<Playerscore>();
            }
            if (string.IsNullOrEmpty(playerscoreState.OrderBy))
            {
                playerscoreState.OrderBy = "PlayerName";
            }

            int pageNo = playerscoreState.CurrentPageNo;
            if (pageNo < 1)
            {
                pageNo = 1;
            }

            SortedDictionary<int, Playerscore> playerscoresDictionary = await GetDictionaryOfPlayerscores(playerscoreState);

            int totalCount = playerscoresDictionary.Count;
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
            List<Playerscore> playerscores = playerscoresDictionary.Skip(recordNo).Take(pageSize).Select(m => m.Value).ToList();

            playerscoreState.CurrentPageNo = pageNo;
            Playerscore firstPlayerscore = playerscores.FirstOrDefault();
            if (firstPlayerscore != null)
            {
                playerscoreState.FirstId = firstPlayerscore.Id;
            }
            else
            {
                playerscoreState.OrgId = 0;
                playerscoreState.OrgPlayerName = "";
                playerscoreState.FirstId = 0;
            }

            return playerscores;
        }

        /// <summary>
        /// Finds the one page of playerscores for one playerscore.
        /// </summary>
        /// <returns>The one page of playerscores for one playerscore.</returns>
        /// <param name="playerscoreState">Playerscore state.</param>
        /// <param name="playerscore">Playerscore.</param>
        /// <param name="id">Identifier.</param>
        public async Task<List<Playerscore>> FindOnePageOfPlayerscoresForOnePlayerscore(PlayerscoreStateOfRequest playerscoreState, Playerscore playerscore, int id)
        {
            if ((playerscoreState == null) || (playerscore == null))
            {
                return new List<Playerscore>();
            }
            if (string.IsNullOrEmpty(playerscoreState.OrderBy))
            {
                playerscoreState.OrderBy = "PlayerName";
            }

            List<Playerscore> playerscores = null;
            KeyValuePair<int, Playerscore> playerscoreWithIndex = new KeyValuePair<int, Playerscore>(-1, null);

            SortedDictionary<int, Playerscore> playerscoresDictionary = await GetDictionaryOfPlayerscores(playerscoreState);

            if (id > 0)
            {
                // There was a selected playerscore
                playerscoreWithIndex = playerscoresDictionary.Where(x => x.Value.Id == id).SingleOrDefault();
            }
            else
            {
                // No selected anguage
                if (playerscoreState.OrderBy == "PlayerName")
                {
                    string playerName = playerscore.PlayerName;
                    playerscoreWithIndex = playerscoresDictionary.Where(x => (String.Compare(x.Value.PlayerName, playerName) >= 0)).FirstOrDefault();
                }
                else if (playerscoreState.OrderBy == "Score")
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
                    playerscoreState.OrgId = 0;
                    playerscoreState.OrgPlayerName = "";
                    playerscoreState.FirstId = 0;
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

            playerscoreState.CurrentPageNo = pageNo;
            playerscoreState.OrgId = playerscore.Id;
            playerscoreState.OrgPlayerName = playerscore.PlayerName;

            Playerscore firstPlayerscore = playerscores.FirstOrDefault();
            if (firstPlayerscore != null)
            {
                playerscoreState.FirstId = firstPlayerscore.Id;
            }
            else
            {
                playerscoreState.FirstId = 0;
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
