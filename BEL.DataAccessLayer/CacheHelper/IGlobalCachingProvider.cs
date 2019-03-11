namespace BEL.DataAccessLayer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// IGlobalCachingProvider interface
    /// </summary>
    public interface IGlobalCachingProvider
    {
        /// <summary>
        /// Adds the item.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        void AddItem(string key, object value);

        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>object Data</returns>
        object GetItem(string key);

        /// <summary>
        /// Gets all keys.
        /// </summary>
        /// <returns>list of string</returns>
        List<string> GetAllKeys();

        /// <summary>
        /// Removes the item.
        /// </summary>
        /// <param name="key">The key.</param>
        void RemoveItem(string key);
    }
}
