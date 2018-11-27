using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VodManageSystem.Models;
using VodManageSystem.Models.DataModels;

namespace VodManageSystem.Utilities
{
    /// <summary>
    /// Some utilities for Json object
    /// </summary>
    public class JsonUtil
    {
        /// <summary>
        /// Gets the object from json string.
        /// </summary>
        /// <returns>The object from json string.</returns>
        /// <param name="song_state">Song state.</param>
        /// <param name="createYn">If set to <c>true</c> create yn.</param>
        /// <typeparam name="T">true--Create new instance if string is null or empty
        /// false--use default value if string is null or empty </typeparam>
        public static T GetObjectFromJsonString<T>(string song_state) where T : class
        {
            T obj = default(T);

            if (!string.IsNullOrEmpty(song_state) )
            {
                obj = JsonConvert.DeserializeObject<T>(song_state);
            }

            return obj;
        }

        /// <summary>
        /// Sets the json string from object.
        /// </summary>
        /// <returns>The json string from object.</returns>
        /// <param name="obj">Object.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static string SetJsonStringFromObject<T>(T obj) where T : class 
        {
            if (obj == null)
            {
                return null;    // return null string
            }
            // Must use Formatting.None
            // Formatting.Indented will call Script error (in View) error 
            // if there is JSON.parse(HtmlString(JSON string)) using jQuery to generate a select list
            return JsonConvert.SerializeObject(obj, Formatting.None, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            });
        }


        public static JObject ConvertSingerToJsongObject(Singer singer)
        {
            JObject jObject = new JObject();
            if (singer == null)
            {
                return jObject;
            }

            jObject.Add("id", singer.Id);
            jObject.Add("singNo", singer.SingNo);
            jObject.Add("singNa", singer.SingNa);
            jObject.Add("sex", singer.Sex);
            jObject.Add("chor", singer.Chor);
            jObject.Add("hot", singer.Hot);
            jObject.Add("numFw", singer.NumFw);
            jObject.Add("numPw", singer.NumPw);
            jObject.Add("picFile", singer.PicFile);

            return jObject;
        }

        public static JObject ConvertSongToJsongObject(Song song)
        {
            JObject jObject = new JObject();
            if (song == null)
            {
                return jObject;
            }

            jObject.Add("id", song.Id);
            jObject.Add("songNo", song.SongNo);
            jObject.Add("songNa", song.SongNa);
            jObject.Add("sNumWord", song.SNumWord);
            jObject.Add("numFw", song.NumFw);
            jObject.Add("numPw", song.NumPw);
            jObject.Add("chor", song.Chor);
            jObject.Add("nMpeg", song.NMpeg);
            jObject.Add("mMpeg", song.MMpeg);
            jObject.Add("vodYn", song.VodYn);
            jObject.Add("vodNo", song.VodNo);
            jObject.Add("pathname", song.Pathname);
            jObject.Add("ordNo", song.OrdNo);
            jObject.Add("orderNum", song.OrderNum);
            jObject.Add("ordOldN", song.OrdOldN);
            jObject.Add("languageId", song.LanguageId);
            if (song.Language != null)
            {
                jObject.Add("languageNo", song.Language.LangNo);
                jObject.Add("languageNa", song.Language.LangNa);
            }
            else
            {
                jObject.Add("languageNo", "");
                jObject.Add("languageNa", "");
            }
            jObject.Add("singer1Id", song.Singer1Id);
            if (song.Singer1 != null)
            {
                jObject.Add("singer1No", song.Singer1.SingNo);
                jObject.Add("singer1Na", song.Singer1.SingNa);
            }
            else
            {
                jObject.Add("singer1No", "");
                jObject.Add("singer1Na", "");
            }
            jObject.Add("singer2Id", song.Singer2Id);
            if (song.Singer2 != null)
            {
                jObject.Add("singer2No", song.Singer2.SingNo);
                jObject.Add("singer2Na", song.Singer2.SingNa);
            }
            else
            {
                jObject.Add("singer2No", "");
                jObject.Add("singer2Na", "");
            }

            jObject.Add("inDate", song.InDate);

            return jObject;
        }

        public static JObject ConvertlanguageToJsongObject(Language language)
        {
            JObject jObject = new JObject();
            if (language == null)
            {
                return jObject;
            }

            jObject.Add("id", language.Id);
            jObject.Add("langNo", language.LangNo);
            jObject.Add("langNa", language.LangNa);
            jObject.Add("langEn", language.LangEn);

            return jObject;
        }

        public static JObject ConvertSingareaToJsongObject(Singarea singarea)
        {
            JObject jObject = new JObject();
            if (singarea == null)
            {
                return jObject;
            }

            jObject.Add("id", singarea.Id);
            jObject.Add("areaNo", singarea.AreaNo);
            jObject.Add("areaNa", singarea.AreaNa);
            jObject.Add("areaEn", singarea.AreaEn);

            return jObject;
        }

        public static JObject ConvertSingerTypeToJsongObject(Singarea singarea, string sex)
        {
            JObject jObject = ConvertSingareaToJsongObject(singarea);
            jObject.Add("sex", sex);

            return jObject;
        }
    }
}
