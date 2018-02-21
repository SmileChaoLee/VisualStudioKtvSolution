using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;

namespace VodManageSystem.ExtensionMethods
{
    /// <summary>
    /// Temp data extensions.
    /// </summary>
    public static class TempDataExtensions
    {
        /// <summary>
        /// Set the specified tempData, key and value.
        /// </summary>
        /// <returns>The set.</returns>
        /// <param name="tempData">Temp data.</param>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static void Set<T>(this ITempDataDictionary tempData, string key, T value) where T : class
        {
            tempData[key] = JsonConvert.SerializeObject(value);
        }

        /// <summary>
        /// Get the specified tempData and key.
        /// </summary>
        /// <returns>The get.</returns>
        /// <param name="tempData">Temp data.</param>
        /// <param name="key">Key.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T Get<T>(this ITempDataDictionary tempData, string key) where T : class
        {
            object o;
            tempData.TryGetValue(key, out o);
            return o == null ? null : JsonConvert.DeserializeObject<T>((string)o);
        }

        /// <summary>
        /// Peek the specified tempData and key.
        /// </summary>
        /// <returns>The peek.</returns>
        /// <param name="tempData">Temp data.</param>
        /// <param name="key">Key.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T Peek<T>(this ITempDataDictionary tempData, string key) where T : class
        {
            object o = tempData.Peek(key);
            return o == null ? null : JsonConvert.DeserializeObject<T>((string)o);
        }
    }
}
