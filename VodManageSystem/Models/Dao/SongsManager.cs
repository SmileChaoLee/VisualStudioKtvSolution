using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using VodManageSystem.Models;
using VodManageSystem.Models.DataModels;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore.Storage;

namespace VodManageSystem.Models.Dao
{
    /// <summary>
    /// a service of Song manager that maintains Song table and its related tables in database
    /// </summary>
    public class SongsManager : IDisposable
    {
        // private members
        private readonly KtvSystemDBContext _context;

        // public members

        /// <summary>
        /// Initializes a new instance of the <see cref="T:VodManageSystem.DOA.SongManager"/> class.
        /// </summary>
        /// <param name="context">Context.</param>
        public SongsManager(KtvSystemDBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Verifies the song.
        /// </summary>
        /// <returns>The song.</returns>
        /// <param name="song">Song.</param>
        private async Task<int> VerifySong(Song song)
        {
            int result = 1; // valid by verification 
            if (song.LanguageId >= 0)
            {
                Language lang = await _context.Language.Where(x => x.Id == song.LanguageId).SingleOrDefaultAsync();
                if (lang == null)
                {
                    // no Language.Id found
                    result = ErrorCodeModel.LanguageNoNotFound;
                    return result;
                }
                song.LanguageId = lang.Id;
            }
            else
            {
                // language id. has to be specified
                result = ErrorCodeModel.LanguageIdIsSpecified;
                return result;
            }
            if (song.Singer1Id >= 0)
            {
                Singer sing1 = await _context.Singer.Where(x => x.Id == song.Singer1Id).SingleOrDefaultAsync();
                if (sing1 == null)
                {
                    // no Singer.SingNo for singer1 found
                    result = ErrorCodeModel.Singer1NoNotFound;
                    return result;
                }
                song.Singer1Id = sing1.Id;
            }
            else
            {
                // song.Singer1Id not specified
                result = ErrorCodeModel.Singer1IdIsNotSpecified;
                return result;
            }
            if (song.Singer2Id >= 0)
            {
                Singer sing2 = await _context.Singer.Where(x => x.Id == song.Singer2Id).SingleOrDefaultAsync();
                if (sing2 == null)
                {
                    // song.Singer2Id not specified
                    result = ErrorCodeModel.Singer2IdIsNotSpecified;
                    return result;
                }
                song.Singer2Id = sing2.Id;
            }
            else
            {
                // no Singer.SingNo for singer2 found
                result = ErrorCodeModel.Singer2NoNotFound;
                return result;
            }
            if (song.VodYn == "Y")
            {
                // must have VodNo and Pathname
                if (string.IsNullOrEmpty(song.VodNo))
                {
                    // Vod No. is empty
                    result = ErrorCodeModel.VodNoOfSongIsEmpty;
                    return result;
                }
                if (string.IsNullOrEmpty(song.Pathname))
                {
                    // Path name is empty
                    result = ErrorCodeModel.PathnameOfVodNoIsEmpty;
                    return result;
                }
            }
            if (song.SNumWord <= 0)
            {
                // number of words cannot be less than 0 or equal to 0
                result = ErrorCodeModel.NumOfWordsLessOrEqualToZero;
            }

            return result;
        }

        /// <summary>
        /// Gets the total page of song table.
        /// </summary>
        /// <returns>The total page of song table.</returns>
        private int[] GetTotalRecordsAndPages(int pageSize)  // by a condition
        {
            int[] result = new int[2] { 0, 0 };

            if (pageSize <= 0)
            {
                Console.WriteLine("The value of pageSize cannot be less than 0.");
                return result;
            }

            int count = _context.Song.Count();
            int totalPages = count / pageSize;
            if ((totalPages * pageSize) != count)
            {
                totalPages++;
            }
            result[0] = count;
            result[1] = totalPages;

            return result;
        }

        private void UpdateStateOfRequest(StateOfRequest mState, Song firstSong, int pageNo, int pageSize, int totalRecords, int totalPages, bool isFind=false)
        {
            mState.CurrentPageNo = pageNo;
            mState.PageSize = pageSize;
            mState.TotalRecords = totalRecords;
            mState.TotalPages = totalPages;
            if (firstSong != null)
            {
                mState.FirstId = firstSong.Id;
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

        private IQueryable<Song> GetAllSongsIQueryable_OLD(StateOfRequest mState)
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

            IQueryable<Song> totalSongs = _context.Song.Include(x => x.Language)
                                          .Include(x => x.Singer1).Include(x => x.Singer2);

            IQueryable<Song> songs;

            string orderByParam = mState.OrderBy.Trim();
            if (orderByParam == "")
            {
                songs = totalSongs;
            }
            else if (orderByParam.Equals("SongNo", StringComparison.OrdinalIgnoreCase))
            {
                songs = totalSongs.OrderBy(x => x.SongNo);
            }
            else if (orderByParam.Equals("SongNa", StringComparison.OrdinalIgnoreCase))
            {
                songs = totalSongs.OrderBy(x => x.SongNa).ThenBy(x => x.SongNo);
            }
            else if (orderByParam.Equals("NumWordsSongNa",StringComparison.OrdinalIgnoreCase))
            {
                songs = totalSongs.OrderBy(x=>x.SNumWord).ThenBy(x => x.SongNa).ThenBy(x => x.SongNo);
            }
            else if (orderByParam.Equals("VodNo", StringComparison.OrdinalIgnoreCase))
            {
                songs = totalSongs.OrderBy(x => x.VodNo).ThenBy(x => x.SongNo);
            }
            else if (orderByParam.Equals("LangSongNa", StringComparison.OrdinalIgnoreCase))
            {
                songs = totalSongs.OrderBy(x => x.Language == null)
                                  .ThenBy(x => x.Language.LangNo + x.SongNa).ThenBy(x => x.SongNo);
            }
            else if (orderByParam.Equals("Singer1Na", StringComparison.OrdinalIgnoreCase))
            {
                songs = totalSongs.OrderBy(x => x.Singer1 == null)
                                  .ThenBy(x => x.Singer1.SingNa).ThenBy(x => x.SongNo);
            }
            else if (orderByParam.Equals("Singer2Na", StringComparison.OrdinalIgnoreCase))
            {
                songs = totalSongs.OrderBy(x => x.Singer2 == null)
                                  .ThenBy(x => x.Singer2.SingNa).ThenBy(x => x.SongNo);
            }
            else
            {
                // not inside range of roder by
                songs = null;   // empty lsit
            }

            if ( (songs != null) && (!string.IsNullOrEmpty(mState.QueryCondition)) )
            {
                string queryString = mState.QueryCondition;
                int plusPos = queryString.IndexOf("+", 0, StringComparison.Ordinal);
                if (plusPos >= 1)
                {
                    // the query condition has two parts
                    // the first one is the field name in song table
                    // the second one is the vaue that the field contains
                    string fieldName = queryString.Substring(0, plusPos).Trim();
                    string fielsSubValue = queryString.Substring(plusPos + 1).Trim();
                    if (fieldName.Equals("SongNo", StringComparison.OrdinalIgnoreCase))
                    {
                        songs = songs.Where(x => x.SongNo.Contains(fielsSubValue));
                    }
                    else if (fieldName.Equals("SongNa", StringComparison.OrdinalIgnoreCase))
                    {
                        songs = songs.Where(x => x.SongNa.Contains(fielsSubValue));
                    }
                }
            }

            return songs;
        }

        private IQueryable<Song> GetAllSongsIQueryableWithoutFilter(StateOfRequest mState)
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

            IQueryable<Song> totalSongs = _context.Song.Include(x => x.Language)
                                          .Include(x => x.Singer1).Include(x => x.Singer2);

            IQueryable<Song> songs;

            string orderByParam = mState.OrderBy.Trim();
            if (orderByParam == "")
            {
                songs = totalSongs;
            }
            else if (orderByParam.Equals("SongNo", StringComparison.OrdinalIgnoreCase))
            {
                songs = totalSongs.OrderBy(x => x.SongNo);
            }
            else if (orderByParam.Equals("SongNa", StringComparison.OrdinalIgnoreCase))
            {
                songs = totalSongs.OrderBy(x => x.SongNa).ThenBy(x => x.SongNo);
            }
            else if (orderByParam.Equals("NumWordsSongNa", StringComparison.OrdinalIgnoreCase))
            {
                songs = totalSongs.OrderBy(x => x.SNumWord).ThenBy(x => x.SongNa).ThenBy(x => x.SongNo);
            }
            else if (orderByParam.Equals("VodNo", StringComparison.OrdinalIgnoreCase))
            {
                songs = totalSongs.OrderBy(x => x.VodNo).ThenBy(x => x.SongNo);
            }
            else if (orderByParam.Equals("LangSongNa", StringComparison.OrdinalIgnoreCase))
            {
                songs = totalSongs.OrderBy(x => x.Language == null)
                                  .ThenBy(x => x.Language.LangNo + x.SongNa).ThenBy(x => x.SongNo);
            }
            else if (orderByParam.Equals("Singer1Na", StringComparison.OrdinalIgnoreCase))
            {
                songs = totalSongs.OrderBy(x => x.Singer1 == null)
                                  .ThenBy(x => x.Singer1.SingNa).ThenBy(x => x.SongNo);
            }
            else if (orderByParam.Equals("Singer2Na", StringComparison.OrdinalIgnoreCase))
            {
                songs = totalSongs.OrderBy(x => x.Singer2 == null)
                                  .ThenBy(x => x.Singer2.SingNa).ThenBy(x => x.SongNo);
            }
            else
            {
                // not inside range of roder by
                songs = null;   // empty lsit
            }

            return songs;
        }

        private IQueryable<Song> GetSongsIQueryableAddFilter(IQueryable<Song> originalSongs, string filter)
        {
            IQueryable<Song> songs = originalSongs;
            if ((originalSongs != null) && (!string.IsNullOrEmpty(filter)))
            {
                string queryString = filter.Trim();
                int plusPos = queryString.IndexOf("+", 0, StringComparison.Ordinal);
                if (plusPos >= 1)
                {
                    // the query condition has two parts
                    // the first one is the field name in song table
                    // the second one is the vaue that the field contains
                    string fieldName = queryString.Substring(0, plusPos).Trim();
                    string fieldSubValue = queryString.Substring(plusPos + 1).Trim();
                    if (!string.IsNullOrEmpty(fieldSubValue))
                    {
                        if (fieldName.Equals("SongNo", StringComparison.OrdinalIgnoreCase))
                        {
                            songs = originalSongs.Where(x => x.SongNo.Contains(fieldSubValue));
                        }
                        else if (fieldName.Equals("SongNa", StringComparison.OrdinalIgnoreCase))
                        {
                            songs = originalSongs.Where(x => x.SongNa.Contains(fieldSubValue));
                        }
                    }
                }
            }

            return songs;
        }

        private IQueryable<Song> GetAllSongsIQueryable(StateOfRequest mState)
        {
            IQueryable<Song> songs = GetAllSongsIQueryableWithoutFilter(mState);
            songs = GetSongsIQueryableAddFilter(songs, mState.QueryCondition);

            return songs;
        }

        // end of private methods

        // public methods
        public List<Song> GetAllSongs(StateOfRequest mState)
        {
            if (mState == null)
            {
                return new List<Song>();    // return empty list
            }
            int pageSize = mState.PageSize;
            if (pageSize <= 0)
            {
                Console.WriteLine("The value of pageSize cannot be less than 0.");
                return new List<Song>();
            }

            mState.CurrentPageNo = -100; // present to get all songs
            List<Song> totalSongs = GetOnePageOfSongs(mState);

            return totalSongs;
        }

        public List<Song> GetOnePageOfSongs(StateOfRequest mState)
        {
            if (mState == null)
            {
                return new List<Song>();
            }
            int pageSize = mState.PageSize;
            if (pageSize <= 0)
            {
                Console.WriteLine("The value of pageSize cannot be less than 0.");
                return new List<Song>();
            }

            IQueryable<Song> totalSongs = GetAllSongsIQueryable(mState);
            if (totalSongs == null)
            {
                return new List<Song>();
            }

            int pageNo = mState.CurrentPageNo;
            int[] returnNumbers = GetTotalRecordsAndPages(pageSize);
            int totalRecords = returnNumbers[0];
            int totalPages = returnNumbers[1];

            // bool getAll = false; // removed on 2018-11-26
            if (pageNo == -1)
            {
                // get the last page
                pageNo = totalPages;
            }
            else if (pageNo == -100)
            {
                // get all songs
                // getAll = true;   // removed on 2018-11-26
                pageNo = 1; // restore pageNo to 1
                pageSize = totalRecords;    // added on 2018-11-26
                totalPages = 1; //  added on 2018-11-26
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

            List<Song> songs = songs = totalSongs.Skip(recordNum).Take(pageSize).ToList();

            UpdateStateOfRequest(mState, songs.FirstOrDefault(), pageNo, pageSize, totalRecords, totalPages);

            return songs;
        }

        public List<Song> GetOnePageOfSongsBySingerId(StateOfRequest mState, int singerId, bool isWebAPI)
        {
            if (mState == null)
            {
                return new List<Song>();
            }
            int pageSize = mState.PageSize;
            if (pageSize <= 0)
            {
                Console.WriteLine("The value of pageSize cannot be less than 0.");
                return new List<Song>();
            }

            IQueryable<Song> totalSongs = GetAllSongsIQueryable(mState);
            if (totalSongs == null)
            {
                return new List<Song>();
            }

            totalSongs = totalSongs.Where(x => (x.Singer1Id == singerId) || (x.Singer2Id == singerId));
            int pageNo = mState.CurrentPageNo;
            int totalRecords = totalSongs.Count();
            int totalPages = totalRecords / pageSize;
            if ( (totalPages * pageSize) != totalRecords)
            {
                totalPages++;
            }

            // bool getAll = false; // removed on 2018-11-26
            if (pageNo == -1)
            {
                // get the last page
                pageNo = totalPages;
            }
            else if (pageNo == -100)
            {
                // get all songs
                // getAll = true;   // removed on 2018-11-26
                pageNo = 1; // restore pageNo to 1
                pageSize = totalRecords;    // added on 2018-11-26
                totalPages = 1; //  added on 2018-11-26
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

            List<Song> songs = totalSongs.Skip(recordNum).Take(pageSize).ToList();

            UpdateStateOfRequest(mState, songs.FirstOrDefault(), pageNo, pageSize, totalRecords, totalPages);

            return songs;
        }

        public List<Song> GetOnePageOfSongsByLanguageIdNumOfWords(StateOfRequest mState, int languageId, int numOfWords, bool isWebAPI)
        {
            if (mState == null)
            {
                return new List<Song>();
            }
            int pageSize = mState.PageSize;
            if (pageSize <= 0)
            {
                Console.WriteLine("The value of pageSize cannot be less than 0.");
                return new List<Song>();
            }

            IQueryable<Song> totalSongs = GetAllSongsIQueryable(mState);
            if (totalSongs == null)
            {
                return new List<Song>();
            }

            if (numOfWords > 0)
            {
                totalSongs = totalSongs.Where(x => (x.LanguageId == languageId) && (x.SNumWord == numOfWords));
            }
            else
            {
                totalSongs = totalSongs.Where(x => x.LanguageId == languageId);
            }
            int pageNo = mState.CurrentPageNo;
            int totalRecords = totalSongs.Count();
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
                // get all songs
                pageNo = 1; // restore pageNo to 1
                pageSize = totalRecords;    // added on 2018-11-26
                totalPages = 1; //  added on 2018-11-26
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

            List<Song> songs = totalSongs.Skip(recordNum).Take(pageSize).ToList();

            UpdateStateOfRequest(mState, songs.FirstOrDefault(), pageNo, pageSize, totalRecords, totalPages);

            return songs;
        }

        public List<Song> GetOnePageOfNewSongByLanguageId(StateOfRequest mState, int languageId, bool isWebAPI)
        {
            if (mState == null)
            {
                return new List<Song>();
            }
            int pageSize = mState.PageSize;
            if (pageSize <= 0)
            {
                Console.WriteLine("The value of pageSize cannot be less than 0.");
                return new List<Song>();
            }

            IQueryable<Song> totalSongs = GetAllSongsIQueryableWithoutFilter(mState);
            if (totalSongs == null)
            {
                return new List<Song>();
            }

            // only take 100 songs
            totalSongs = totalSongs.Where(x => x.LanguageId == languageId).OrderByDescending(x => x.InDate).Take(100);
            totalSongs = GetSongsIQueryableAddFilter(totalSongs, mState.QueryCondition);

            int pageNo = mState.CurrentPageNo;
            int totalRecords = totalSongs.Count();
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
                // get all songs
                pageNo = 1; // restore pageNo to 1
                pageSize = totalRecords;    // added on 2018-11-26
                totalPages = 1; //  added on 2018-11-26
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

            List<Song> songs = totalSongs.ToList().Skip(recordNum).Take(pageSize).ToList();

            UpdateStateOfRequest(mState, songs.FirstOrDefault(), pageNo, pageSize, totalRecords, totalPages);

            return songs;
        }

        public List<Song> GetOnePageOfHotSongByLanguageId(StateOfRequest mState, int languageId, bool isWebAPI)
        {
            if (mState == null)
            {
                return new List<Song>();
            }
            int pageSize = mState.PageSize;
            if (pageSize <= 0)
            {
                Console.WriteLine("The value of pageSize cannot be less than 0.");
                return new List<Song>();
            }

            IQueryable<Song> totalSongs = GetAllSongsIQueryableWithoutFilter(mState);
            if (totalSongs == null)
            {
                return new List<Song>();
            }

            // only take 100 songs
            totalSongs = totalSongs.Where(x => x.LanguageId == languageId).OrderByDescending(x=>x.OrderNum).Take(100);
            totalSongs = GetSongsIQueryableAddFilter(totalSongs, mState.QueryCondition);

            int pageNo = mState.CurrentPageNo;
            int totalRecords = totalSongs.Count();
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
                // get all songs
                pageNo = 1; // restore pageNo to 1
                pageSize = totalRecords;    // added on 2018-11-26
                totalPages = 1; //  added on 2018-11-26
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

            List<Song> songs = totalSongs.ToList().Skip(recordNum).Take(pageSize).ToList();

            UpdateStateOfRequest(mState, songs.FirstOrDefault(), pageNo, pageSize, totalRecords, totalPages);

            return songs;
        }

        /// <summary>
        /// Finds the one page of songs for one song.
        /// </summary>
        /// <returns>The one page of songs for one song.</returns>
        /// <param name="mState">Song state.</param>
        /// <param name="song">Song.</param>
        /// <param name="id">Identifier.</param>
        public List<Song> FindOnePageOfSongsForOneSong(StateOfRequest mState, Song song, int id)
        {
            if ((mState == null) || (song == null))
            {
                return new List<Song>();
            }
            int pageSize = mState.PageSize;
            if (pageSize <= 0)
            {
                Console.WriteLine("The value of pageSize cannot be less than 0.");
                return new List<Song>();
            }

            IQueryable<Song> totalSongs = GetAllSongsIQueryable(mState);
            if (totalSongs == null)
            {
                return new List<Song>();
            }

            List<Song> songs = null;
            Song songWithIndex = null;
            IQueryable<Song> songsTempList = null;

            string orderByParam = mState.OrderBy.Trim();
            if (id >= 0)
            {
                // There was a song selected
                songsTempList = totalSongs.Where(x => x.Id == id);
            }
            else
            {
                // No song selected
                if (orderByParam == "")
                {
                    // order by Id
                    int song_id = song.Id;
                    songsTempList = totalSongs.Where(x => (x.Id == song_id));
                }
                else if (orderByParam.Equals("SongNo", StringComparison.OrdinalIgnoreCase))
                {
                    string song_no = song.SongNo.Trim();
                    int len = song_no.Length;
                    songsTempList = totalSongs.Where(x => x.SongNo.Trim().Substring(0, len) == song_no);
                }
                else if (orderByParam.Equals("SongNa",StringComparison.OrdinalIgnoreCase))
                {
                    string song_na = song.SongNa.Trim();
                    int len = song_na.Length;
                    songsTempList = totalSongs.Where(x => x.SongNa.Trim().Substring(0, len) == song_na);
                }
                else if (orderByParam.Equals("VodNo", StringComparison.OrdinalIgnoreCase))
                {
                    string vod_no = song.VodNo.Trim();
                    int len = vod_no.Length;
                    songsTempList = totalSongs.Where(x => x.VodNo.Trim().Substring(0, len) == vod_no);
                }
                else if (orderByParam.Equals("LangSongNa", StringComparison.OrdinalIgnoreCase))
                {
                    string lang_no = song.Language.LangNo;
                    if (string.IsNullOrEmpty(lang_no) )
                    {
                        lang_no = _context.Language.FirstOrDefault().LangNo;
                    }
                    string song_na = song.SongNa.Trim();
                    int len = song_na.Length;
                    songsTempList = totalSongs.Where(x => (x.Language != null)
                         && (x.Language.LangNo + x.SongNa.Trim().Substring(0, len) == lang_no + song_na));
                }
                else if (orderByParam.Equals("Singer1Na", StringComparison.OrdinalIgnoreCase))
                {
                    string singer1Na = song.Singer1.SingNa.Trim();
                    int len = singer1Na.Length;
                    songsTempList = totalSongs.Where(x => (x.Singer1 != null)
                         && (x.Singer1.SingNa.Trim().Substring(0, len) == singer1Na));
                }
                else if (orderByParam.Equals("Singer2Na", StringComparison.OrdinalIgnoreCase))
                {
                    string singer2Na = song.Singer2.SingNa.Trim();
                    int len = singer2Na.Length;
                    songsTempList = totalSongs.Where(x => (x.Singer2 != null)
                         && (x.Singer2.SingNa.Trim().Substring(0, len) == singer2Na));
                }
                else
                {
                    // not inside range of roder by then return empty lsit
                    return new List<Song>();
                }
            }

            int totalRecords = totalSongs.Count();  // the whole song table

            bool isFound = true;
            songWithIndex = songsTempList.FirstOrDefault(); // the first one found
            if (songWithIndex == null)
            {
                isFound = false;    // song that was assigned is not found
                if (totalRecords == 0)
                {
                    // Song Table is empty
                    UpdateStateOfRequest(mState, songWithIndex, mState.CurrentPageNo, pageSize, 0, 0, true);
                    // return empty list
                    return new List<Song>();
                }
                else
                {
                    // go to last page
                    songWithIndex = totalSongs.LastOrDefault();
                }
            }

            song.CopyFrom(songWithIndex);

            // find the row number of songWithIndex
            int tempCount = 0;
            foreach (var songVar in totalSongs)
            {
                ++tempCount;    // first row number is 1
                if (songVar.Id == songWithIndex.Id)
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

            songs = totalSongs.Skip(recordNo).Take(pageSize).ToList();

            int totalPages = totalRecords / pageSize;
            if ((totalPages * pageSize) != totalRecords)
            {
                totalPages++;
            }

            if (isFound)
            {
                // found
                mState.OrgId = song.Id; // chnaged OrgId to the song id found
            }
            else
            {
                // not found, then it is last page and last record
                mState.OrgId = 0;   // no song is selected
            }
            UpdateStateOfRequest(mState, songs.FirstOrDefault(), pageNo, pageSize, totalRecords, totalPages, true);

            return songs;
        }

        /// <summary>
        /// Finds the one song by song no.
        /// </summary>
        /// <returns>The one song by song no.</returns>
        /// <param name="song_no">Song no.</param>
        public async Task<Song> FindOneSongBySongNo(string song_no)
        {
            Song song = await _context.Song.Where(x=>x.SongNo == song_no).Include(x=>x.Language)
                            .Include(x=>x.Singer1).Include(x=>x.Singer2).SingleOrDefaultAsync();

            return song;
        }

        /// <summary>
        /// Finds the one song by identifier.
        /// </summary>
        /// <returns>The one song by identifier (Song.Id).</returns>
        /// <param name="id">the id of the song.</param>
        public async Task<Song> FindOneSongById(int id)
        {
            // find a song from context
            Song song = await _context.Song.Where(x=>x.Id == id).Include(x=>x.Language)
                        .Include(x=>x.Singer1).Include(x=>x.Singer2).SingleOrDefaultAsync();

            return song;
        }

        /// <summary>
        /// Adds the one song to table.
        /// </summary>
        /// <returns>Return the error code.</returns>
        /// <param name="song">Song.</param>
        public async Task<int> AddOneSongToTable(Song song)
        {
            int result = ErrorCodeModel.ErrorBecauseBugs;
            if (song == null)
            {
                // the data for updating is empty
                result = ErrorCodeModel.SongIsNull;
                return result;
            }
            if (string.IsNullOrEmpty(song.SongNo))
            {
                // the song no that input by user is empty
                result = ErrorCodeModel.SongNoIsEmpty;
                return result;
            }
            Song oldSong = await FindOneSongBySongNo(song.SongNo);
            if (oldSong != null)
            {
                // song_no is duplicate
                result = ErrorCodeModel.SongNoDuplicate;
                return result;
            }

            // verifying the validation for song data
            int validCode = await VerifySong(song);
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
                    _context.Add(song);
                    await _context.SaveChangesAsync();
                    dbTransaction.Commit();
                    result = ErrorCodeModel.Succeeded;
                }
                catch (DbUpdateException ex)
                {
                    string errorMsg = ex.ToString();
                    Console.WriteLine("Failed to add one song: \n" + errorMsg);
                    dbTransaction.Rollback();
                    result = ErrorCodeModel.DatabaseError;
                }
            }

            return result;
        }

        /// <summary>
        /// Updates the one song by identifier.
        /// </summary>
        /// <returns>Return the error code</returns>
        /// <param name="id">Identifier.</param>
        /// <param name="song">Song.</param>
        public async Task<int> UpdateOneSongById(int id, Song song)
        {
            int result = ErrorCodeModel.ErrorBecauseBugs;
            if (id == 0)
            {
                // its a bug, id of song cannot be 0
                result = ErrorCodeModel.ErrorBecauseBugs;
                return result;
            }
            if (song == null)
            {
                // the data for updating is empty
                result = ErrorCodeModel.SongIsNull;
                return result;
            }
            if (string.IsNullOrEmpty(song.SongNo))
            {
                // the song no that input by user is empty
                result = ErrorCodeModel.SongNoIsEmpty;
                return result;
            }
            Song newSong = await FindOneSongBySongNo(song.SongNo);
            if (newSong != null)
            {
                if (newSong.Id != id)
                {
                    // song no is duplicate
                    result = ErrorCodeModel.SongNoDuplicate;
                    return result;
                }
            }

            Song orgSong = await FindOneSongById(id);
            if (orgSong == null)
            {
                // the original song does not exist any more
                result = ErrorCodeModel.OriginalSongNotExist;
                return result;
            }
            else
            {
                orgSong.CopyColumnsFrom(song);
                
                // verifying the validation for Song data
                int validCode = await VerifySong(orgSong);
                if (validCode != ErrorCodeModel.Succeeded)
                {
                    // data is invalid
                    result = validCode;
                    return result;
                }
                
                // check if entry state changed
                if ( (_context.Entry(orgSong).State) == EntityState.Modified)
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
                            Console.WriteLine("Failed to update song table: \n" + msg);
                            dbTransaction.Rollback();
                            result = ErrorCodeModel.DatabaseError;
                        }
                    }
                }
                else
                {
                    result = ErrorCodeModel.SongNotChanged; // no changed
                }
            }

            return result;
        }

        /// <summary>
        /// Deletes the one song by song no.
        /// </summary>
        /// <returns>Return the error code.</returns>
        /// <param name="song_no">Song no.</param>
        public async Task<int> DeleteOneSongBySongNo(string song_no)
        {
            int result = ErrorCodeModel.ErrorBecauseBugs;
            if (string.IsNullOrEmpty(song_no))
            {
                // its a bug, the original song no is empty
                result = ErrorCodeModel.OriginalSongNoIsEmpty;
                return result;
            }

            Song orgSong = await FindOneSongBySongNo(song_no);
            if (orgSong == null)
            {
                // the original song does not exist any more
                result = ErrorCodeModel.OriginalSongNotExist;
            }
            else
            {
                using (var dbTransaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        _context.Song.Remove(orgSong);
                        await _context.SaveChangesAsync();
                        dbTransaction.Commit();
                        result = ErrorCodeModel.Succeeded; // succeeded to update
                    }
                    catch (DbUpdateException ex)
                    {
                        string msg = ex.ToString();
                        Console.WriteLine("Failed to delete one song. Please see log file.\n" + msg);
                        dbTransaction.Rollback();
                        result = ErrorCodeModel.DatabaseError;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Deletes the one song by identifier.
        /// </summary>
        /// <returns>Return the error code.</returns>
        /// <param name="id">Identifier.</param>
        public async Task<int> DeleteOneSongById(int id)
        {
            int result = ErrorCodeModel.ErrorBecauseBugs;
            if (id == 0)
            {
                // its a bug, the id of song cannot be 0
                result = ErrorCodeModel.ErrorBecauseBugs;
                return result;
            }

            Song orgSong = await FindOneSongById(id);
            if (orgSong == null)
            {
                // the original song does not exist any more
                result = ErrorCodeModel.OriginalSongNotExist;
            }
            else
            {
                using (var dbTransaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        _context.Song.Remove(orgSong);
                        await _context.SaveChangesAsync();
                        dbTransaction.Commit();
                        result = ErrorCodeModel.Succeeded; // succeeded to update
                    }
                    catch (DbUpdateException ex)
                    {
                        string msg = ex.ToString();
                        Console.WriteLine("Failed to delete one song. Please see log file.\n" + msg);
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
        // ~SongManager() {
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
