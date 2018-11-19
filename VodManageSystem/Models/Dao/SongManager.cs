using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using VodManageSystem.Models;
using VodManageSystem.Models.DataModels;
using System.Xml.Linq;

namespace VodManageSystem.Models.Dao
{
    /// <summary>
    /// a service of Song manager that maintains Song table and its related tables in database
    /// </summary>
    public class SongManager : IDisposable
    {
        // private members
        private readonly KtvSystemDBContext _context;

        // public members

        /// <summary>
        /// Initializes a new instance of the <see cref="T:VodManageSystem.DOA.SongManager"/> class.
        /// </summary>
        /// <param name="context">Context.</param>
        public SongManager(KtvSystemDBContext context)
        {
            _context = context;
        }

        // private methods
        /// <summary>
        /// Copies the song from exist one.
        /// </summary>
        /// <returns>The song from exist one.</returns>
        /// <param name="song">Song.</param>
        /// <param name="existSong">Exist song.</param>
        private async Task CopySongFromExistOne(Song song, Song existSong)
        {
            if (existSong != null)
            {
                Language lang = await _context.Language.Where(x =>x.Id==existSong.LanguageId).SingleOrDefaultAsync();
                if (lang != null)
                {
                    existSong.Language = lang;
                    existSong.LangNo = lang.LangNo;
                    existSong.LangNa = lang.LangNa;
                }
                Singer sing1 = await _context.Singer.Where(x=>x.Id==existSong.Singer1Id).SingleOrDefaultAsync();
                if (sing1 != null)
                {
                    existSong.Singer1 = sing1;
                    existSong.Singer1No = sing1.SingNo;
                    existSong.Singer1Na = sing1.SingNa;
                }
                Singer sing2 = await _context.Singer.Where(x=>x.Id==existSong.Singer2Id).SingleOrDefaultAsync();
                if (sing2 != null){
                    existSong.Singer2 = sing2;
                    existSong.Singer2No = sing2.SingNo;
                    existSong.Singer2Na = sing2.SingNa;
                }

                song.CopyFrom(existSong);
            }
        }

        /// <summary>
        /// Verifies the song.
        /// </summary>
        /// <returns>The song.</returns>
        /// <param name="song">Song.</param>
        private async Task<int> VerifySong(Song song)
        {
            int result = 1; // valid by verification 
            if (string.IsNullOrEmpty(song.LangNo))
            {
                // language no. has to be specified
                result = ErrorCodeModel.LanguageNoIsEmpty;
                return result;
            }
            else
            {
                Language lang = await _context.Language.Where(x => x.LangNo == song.LangNo).SingleOrDefaultAsync();
                if (lang != null)
                {
                    song.LanguageId = lang.Id;
                }
                else
                {
                    // no Language.LangNo found
                    result = ErrorCodeModel.LanguageNoNotFound;
                    return result;
                }
            }
            if (!string.IsNullOrEmpty(song.Singer1No))
            {
                Singer sing1 = await _context.Singer.Where(x => x.SingNo == song.Singer1No).SingleOrDefaultAsync();
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
                // no Singer.SingNo for singer1 found
                result = ErrorCodeModel.Singer1NoNotFound;
                return result;
            }
            if (!string.IsNullOrEmpty(song.Singer2No))
            {
                Singer sing2 = await _context.Singer.Where(x => x.SingNo == song.Singer2No).SingleOrDefaultAsync();
                if (sing2 == null)
                {
                    // no Singer.SingNo for singer2 found
                    result = ErrorCodeModel.Singer2NoNotFound;
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
            return result;
        }

        // end of private methods

        // public methods
        public async Task<List<Song>> GetAllSongs(StateOfRequest mState)
        {
            if (mState == null)
            {
                return new List<Song>();    // return empty list
            }

            mState.CurrentPageNo = -100; // present to get all songs
            List<Song> totalSongs = await GetOnePageOfSongs(mState);

            return totalSongs;

        }

        public async Task<List<Song>> GetOnePageOfSongs(StateOfRequest mState)
        {
            if (mState == null)
            {
                return new List<Song>();
            }

            if (string.IsNullOrEmpty(mState.OrderBy))
            {
                // default is order by singer's No
                mState.OrderBy = "SongNo";
            }

            var songsList = _context.Song.Where(x => x.Id == 0);
            if (mState.OrderBy == "SongNo")
            {
                songsList = _context.Song.Include(x => x.Language)
                                    .Include(x => x.Singer1).Include(x => x.Singer2)
                                    .OrderBy(x => x.SongNo);
            }
            else if (mState.OrderBy == "SongNa")
            {
                songsList = _context.Song.Include(x => x.Language)
                                           .Include(x => x.Singer1).Include(x => x.Singer2)
                                      .OrderBy(x => x.SongNa).ThenBy(x => x.SongNo);
            }
            else if (mState.OrderBy == "VodNo")
            {
                songsList = _context.Song.Include(x => x.Language)
                                    .Include(x => x.Singer1).Include(x => x.Singer2)
                                    .OrderBy(x => x.VodNo).ThenBy(x => x.SongNo);
            }
            else if (mState.OrderBy == "LangSongNa")
            {
                songsList = _context.Song.Include(x => x.Language)
                                    .Include(x => x.Singer1).Include(x => x.Singer2)
                                    .OrderBy(x => x.Language == null)
                                    .ThenBy(x => x.Language.LangNo + x.SongNa).ThenBy(x => x.SongNo);
            }
            else if (mState.OrderBy == "Singer1Na")
            {
                songsList = _context.Song.Include(x => x.Language)
                                     .Include(x => x.Singer1).Include(x => x.Singer2)
                                     .OrderBy(x => x.Singer1 == null)
                                    .ThenBy(x => x.Singer1.SingNa).ThenBy(x => x.SongNo);
            }
            else if (mState.OrderBy == "Singer2Na")
            {
                songsList = _context.Song.Include(x => x.Language)
                                    .Include(x => x.Singer1).Include(x => x.Singer2)
                                    .OrderBy(x => x.Singer2 == null)
                                    .ThenBy(x => x.Singer2.SingNa).ThenBy(x => x.SongNo);
            }
            else
            {
                songsList = _context.Song.Include(x => x.Language)
                                    .Include(x => x.Singer1).Include(x => x.Singer2);
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

            List<Song> songs;
            if (getAll)
            {
                // get all songs
                songs = await songsList.AsNoTracking().ToListAsync();
            }
            else
            {
                songs = await songsList.Skip(recordNum).Take(pageSize).AsNoTracking().ToListAsync();
            }

            mState.CurrentPageNo = pageNo;
            mState.PageSize = pageSize;
            mState.TotalRecords = totalRecords;
            mState.TotalPages = totalPages;

            Song firstSong = songs.FirstOrDefault();
            if (firstSong != null)
            {
                mState.FirstId = firstSong.Id;
            }
            else
            {
                mState.OrgId = 0;
                mState.OrgNo = "";
                mState.FirstId = 0;
            }

            return songs;
        }

        /// <summary>
        /// Gets the dictionary of songs.
        /// </summary>
        /// <returns>The dictionary of songs.</returns>
        /// <param name="mState">Song state.</param>
        public async Task<SortedDictionary<int,Song>> GetDictionaryOfSongs(StateOfRequest mState)
        {
            if (mState == null)
            {
                return new SortedDictionary<int,Song>();
            }

            List<Song> totalSongs = await _context.Song.Include(x=>x.Language)
                        .Include(x=>x.Singer1).Include(x=>x.Singer2)
                        .AsNoTracking().ToListAsync();
            
            Dictionary<int,Song> songsDictionary = null;

            if (mState.OrderBy == "SongNo")
            {
                songsDictionary = totalSongs.OrderBy(x => x.SongNo)
                            .Select( (m,index) => new {rowNumber = index+1, m})
                            .ToDictionary(m=>m.rowNumber, m=>m.m);
            }
            else if (mState.OrderBy == "SongNa")
            {
                songsDictionary = totalSongs.OrderBy(x => x.SongNa).ThenBy(x=>x.SongNo)
                            .Select( (m,index) => new {rowNumber = index+1, m})
                            .ToDictionary(m=>m.rowNumber, m=>m.m);
            }
            else if (mState.OrderBy == "VodNo")
            {
                songsDictionary = totalSongs.OrderBy(x => x.VodNo).ThenBy(x=>x.SongNo)
                            .Select( (m,index) => new {rowNumber = index+1, m})
                            .ToDictionary(m=>m.rowNumber, m=>m.m);
            }
            else if (mState.OrderBy == "LangSongNa")
            {
                songsDictionary = totalSongs.OrderBy(x => x.Language==null)
                            .ThenBy(x=>x.Language.LangNo+x.SongNa).ThenBy(x=>x.SongNo)
                            .Select( (m,index) => new {rowNumber = index+1, m})
                            .ToDictionary(m=>m.rowNumber, m=>m.m);
            }
            else if (mState.OrderBy == "Singer1Na")
            {
                songsDictionary = totalSongs.OrderBy(x => x.Singer1==null)
                            .ThenBy(x=>x.Singer1.SingNa).ThenBy(x=>x.SongNo)
                            .Select( (m,index) => new {rowNumber = index+1, m})
                            .ToDictionary(m=>m.rowNumber, m=>m.m);

            }
            else if (mState.OrderBy == "Singer2Na")
            {
                songsDictionary = totalSongs.OrderBy(x => x.Singer2==null)
                            .ThenBy(x=>x.Singer2.SingNa).ThenBy(x=>x.SongNo)
                            .Select( (m,index) => new {rowNumber = index+1, m})
                            .ToDictionary(m=>m.rowNumber, m=>m.m);
            }
            else
            {
                // not inside range of roder by
                songsDictionary = new Dictionary<int, Song>();    // empty lsit
            }

            return new SortedDictionary<int,Song>(songsDictionary);
        }

        /// <summary>
        /// Gets the total page of song table.
        /// </summary>
        /// <returns>The total page of song table.</returns>
        public async Task<int[]> GetTotalRecordsAndPages(int pageSize)  // by a condition
        {
            int[] result = new int[2] { 0, 0 };

            if (pageSize <= 0)
            {
                Console.WriteLine("The value of pageSize cannot be less than 0.");
                return result;
            }

            int count = await _context.Song.CountAsync();
            int totalPages = count / pageSize;
            if ( (totalPages * pageSize) != count )
            {
                totalPages++;
            }
            result[0] = count;
            result[1] = totalPages;

            return result;
        }

        // public methods

        /// <summary>
        /// Gets the one page of songs dictionary.
        /// </summary>
        /// <returns>The one page of songs dictionary.</returns>
        /// <param name="mState">Song state.</param>
        public async Task<List<Song>> GetOnePageOfSongsDictionary(StateOfRequest mState)
        {
            if (mState == null)
            {
                return new List<Song>();
            }
            if (string.IsNullOrEmpty(mState.OrderBy))
            {
                mState.OrderBy = "SongNo";
            }

            SortedDictionary<int, Song> songsDictionary = await GetDictionaryOfSongs(mState);

            int pageNo = mState.CurrentPageNo;
            int pageSize = mState.PageSize;
            int totalRecords = songsDictionary.Count;
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

            List<Song> songs;
            if (getAll)
            {
                songs = songsDictionary.Select(m => m.Value).ToList();
            }
            else
            {
                songs = songsDictionary.Skip(recordNo).Take(pageSize).Select(m => m.Value).ToList();
            }

            mState.CurrentPageNo = pageNo;
            mState.PageSize = pageSize;
            mState.TotalRecords = totalRecords;
            mState.TotalPages = totalPages;

            Song firstSong = songs.FirstOrDefault();
            if (firstSong != null)
            {
                mState.FirstId = firstSong.Id;
            }
            else
            {
                mState.OrgId = 0;
                mState.OrgNo = "";
                mState.FirstId = 0;
            }

            return songs;
        }

        /// <summary>
        /// Finds the one page of songs for one song.
        /// </summary>
        /// <returns>The one page of songs for one song.</returns>
        /// <param name="mState">Song state.</param>
        /// <param name="song">Song.</param>
        /// <param name="id">Identifier.</param>
        public async Task<List<Song>> FindOnePageOfSongsForOneSong(StateOfRequest mState, Song song, int id)
        {
            if ( (mState == null) || (song == null) )
            {
                return new List<Song>();
            }
            if (string.IsNullOrEmpty(mState.OrderBy))
            {
                mState.OrderBy = "SongNo";
            }

            int pageSize = mState.PageSize;

            List<Song> songs = null;
            KeyValuePair<int,Song> songWithIndex = new KeyValuePair<int, Song>(-1,null);

            SortedDictionary<int, Song> songsDictionary = await GetDictionaryOfSongs(mState);

            if (id > 0)
            {
                // There was a selected song
                songWithIndex = songsDictionary.Where(x=>x.Value.Id == id).SingleOrDefault();
            }
            else
            {
                // No selected song
                if (mState.OrderBy == "SongNo")
                {
                    string song_no = song.SongNo;
                    songWithIndex = songsDictionary.Where(x=>(String.Compare(x.Value.SongNo,song_no)>=0)).FirstOrDefault();
                }
                else if (mState.OrderBy == "SongNa")
                {
                    string song_na = song.SongNa;
                    songWithIndex = songsDictionary.Where(x=>(String.Compare(x.Value.SongNa,song_na)>=0)).FirstOrDefault();
                }
                else if (mState.OrderBy == "VodNo")
                {
                    string vod_no = song.VodNo;
                    songWithIndex = songsDictionary.Where(x=>(String.Compare(x.Value.VodNo,vod_no)>=0)).FirstOrDefault();
                }
                else if (mState.OrderBy == "LangSongNa")
                {
                    string lang_no = song.LangNo;
                    string song_na = song.SongNa;
                    songWithIndex = songsDictionary.Where( x => {
                            bool YN = true;
                            if (!string.IsNullOrEmpty(lang_no))
                            {
                                if (x.Value.Language != null)
                                {
                                    YN = (String.Compare(x.Value.Language.LangNo+x.Value.SongNa,lang_no+song_na)>=0);
                                }
                                else
                                {
                                    YN = false;
                                }
                            }    
                            return YN;
                    }).FirstOrDefault();
                }
                else if (mState.OrderBy == "Singer1Na")
                {
                    string singer1Na = song.Singer1Na;
                    songWithIndex = songsDictionary.Where( x => {
                            bool YN = true;
                            if (!string.IsNullOrEmpty(singer1Na))
                            {
                                if (x.Value.Singer1 != null)
                                {
                                    YN = (String.Compare(x.Value.Singer1.SingNa,singer1Na)>=0);
                                }
                                else
                                {
                                    YN = false;
                                }
                            }
                            return YN;
                    }).FirstOrDefault();
                }
                else if (mState.OrderBy == "Singer2Na")
                {
                    string singer2Na = song.Singer2Na;
                    songWithIndex = songsDictionary.Where( x => {
                            bool YN = true;
                            if (!string.IsNullOrEmpty(singer2Na))
                            {
                                if (x.Value.Singer2 != null)
                                {
                                    YN = (String.Compare(x.Value.Singer2.SingNa,singer2Na)>=0);
                                }
                                else
                                {
                                    YN = false;
                                }
                            }
                            return YN;
                    }).FirstOrDefault();
                }
                else
                {
                    // not inside range of roder by then return empty lsit
                    return new List<Song>(); 
                }
            }

            if (songWithIndex.Value == null)
            {
                if (songsDictionary.Count == 0)
                {
                    // dictionay (Song Table) is empty
                    mState.OrgId = 0;
                    mState.OrgNo = "";
                    mState.FirstId = 0;
                    // return empty list
                    return new List<Song>();
                }
                else
                {
                    // go to last page
                    songWithIndex = songsDictionary.LastOrDefault();
                }
            }
        
            Song songFound = songWithIndex.Value;
            // await CopySongFromExistOne(song, songFound);
            song.CopyFrom(songFound);

            int tempCount = songWithIndex.Key;
            int pageNo =  tempCount / pageSize;
            if ( (pageNo * pageSize) != tempCount)
            {
                pageNo++;
            }
            int recordNo = (pageNo - 1) * pageSize;
            songs = songsDictionary.Skip(recordNo).Take(pageSize).Select(m=>m.Value).ToList();

            int totalRecords = songsDictionary.Count;
            int totalPages = totalRecords / pageSize;
            if ( (totalPages * pageSize) != totalRecords )
            {
                totalPages++;
            }

            mState.CurrentPageNo = pageNo;
            mState.PageSize = pageSize;
            mState.TotalRecords = totalRecords; 
            mState.TotalPages = totalPages;
            mState.OrgId = song.Id;
            mState.OrgNo = song.SongNo;

            Song firstSong = songs.FirstOrDefault();
            if(firstSong != null)
            {
                mState.FirstId = firstSong.Id;
            }
            else
            {
                mState.FirstId = 0;
            }
               
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
                if (song != null)
                {
                    // store other values associated with this song
                    song.LangNo = "";
                    song.LangNa = "";
                    if (song.Language != null)
                    {
                        song.LangNo = song.Language.LangNo;
                        song.LangNa = song.Language.LangNa;
                    }
                    song.Singer1No = "";
                    song.Singer1Na = "";
                    if (song.Singer1 != null)
                    {
                        song.Singer1No = song.Singer1.SingNo;
                        song.Singer1Na = song.Singer1.SingNa;
                    }
                    song.Singer2No = "";
                    song.Singer2Na = "";
                    if (song.Singer2 != null)
                    {
                        song.Singer2No = song.Singer2.SingNo;
                        song.Singer2Na = song.Singer2.SingNa;
                    }
                }

            return song;
        }

        /// <summary>
        /// Finds the one song by identifier.
        /// </summary>
        /// <returns>The one song by identifier (Song.Id).</returns>
        /// <param name="id">the id of the song.</param>
        public async Task<Song> FindOneSongById(int id)
        {
            Song song = null;
                // find a song from context
            song = await _context.Song.Where(x=>x.Id == id).Include(x=>x.Language)
                        .Include(x=>x.Singer1).Include(x=>x.Singer2).SingleOrDefaultAsync();
            
            if (song != null)
            {
                // store other values associated with this song
                song.LangNo = "";
                song.LangNa = "";
                if (song.Language != null)
                {
                    song.LangNo = song.Language.LangNo;
                    song.LangNa = song.Language.LangNa;
                }
                song.Singer1No = "";
                song.Singer1Na = "";
                if (song.Singer1 != null)
                {
                    song.Singer1No = song.Singer1.SingNo;
                    song.Singer1Na = song.Singer1.SingNa;
                }
                song.Singer2No = "";
                song.Singer2Na = "";
                if (song.Singer2 != null)
                {
                    song.Singer2No = song.Singer2.SingNo;
                    song.Singer2Na = song.Singer2.SingNa;
                }
            }

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

            try
            {
                // verifying the validation for song data
                int validCode = await VerifySong(song);
                if (validCode != ErrorCodeModel.Succeeded)
                {
                    // data is invalid
                    result = validCode;
                    return result;
                }

                _context.Add(song);
                await _context.SaveChangesAsync();
                result = ErrorCodeModel.Succeeded;
            }
            catch (DbUpdateException ex)
            {
                string errorMsg = ex.ToString();
                Console.WriteLine("Failed to add one song: \n" + errorMsg);
                result = ErrorCodeModel.DatabaseError;    
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

            try
            {
                Song orgSong = await FindOneSongById(id);
                if (orgSong == null)
                {
                    // the original song does not exist any more
                    result = ErrorCodeModel.OriginalSongNotExist;
                    return result;
                }
                else
                {
                    /*
                    orgSong.SongNo = song.SongNo;
                    orgSong.SongNa = song.SongNa;
                    orgSong.MMpeg = song.MMpeg;
                    orgSong.NMpeg = song.NMpeg;
                    orgSong.NumFw = song.NumFw;
                    orgSong.NumPw = song.NumPw;
                    orgSong.Chor = song.Chor;
                    orgSong.VodYn = song.VodYn;
                    orgSong.VodNo = song.VodNo;
                    orgSong.Pathname = song.Pathname;
                    orgSong.InDate = song.InDate;
                    orgSong.LangNo = song.LangNo;
                    orgSong.Singer1No = song.Singer1No;
                    orgSong.Singer2No = song.Singer2No;
                    */

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
                        await _context.SaveChangesAsync();
                        result = ErrorCodeModel.Succeeded; // succeeded to update
                    }
                    else
                    {
                        result = ErrorCodeModel.SongNotChanged; // no changed
                    }
                }
            }
            catch (DbUpdateException ex)
            {
                string msg = ex.ToString();
                Console.WriteLine("Failed to update song table: \n" + msg);
                result = ErrorCodeModel.DatabaseError;
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
            try
            {
                Song orgSong = await FindOneSongBySongNo(song_no);
                if (orgSong == null)
                {
                    // the original song does not exist any more
                    result = ErrorCodeModel.OriginalSongNotExist;
                }
                else
                {
                    _context.Song.Remove(orgSong);
                    await _context.SaveChangesAsync();
                    result = ErrorCodeModel.Succeeded; // succeeded to update
                }
            }
            catch (DbUpdateException ex)
            {
                string msg = ex.ToString();
                Console.WriteLine("Failed to delete one song. Please see log file.\n" + msg);
                result = ErrorCodeModel.DatabaseError;
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
            try
            {
                Song orgSong = await FindOneSongById(id);
                if (orgSong == null)
                {
                    // the original song does not exist any more
                    result = ErrorCodeModel.OriginalSongNotExist;
                }
                else
                {
                    _context.Song.Remove(orgSong);
                    await _context.SaveChangesAsync();
                    result = ErrorCodeModel.Succeeded; // succeeded to update
                }
            }
            catch (DbUpdateException ex)
            {
                string msg = ex.ToString();
                Console.WriteLine("Failed to delete one song. Please see log file.\n" + msg);
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
