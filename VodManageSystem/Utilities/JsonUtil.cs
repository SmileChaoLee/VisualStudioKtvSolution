using System;
using Newtonsoft.Json;

using VodManageSystem.Models;

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
    }
}
