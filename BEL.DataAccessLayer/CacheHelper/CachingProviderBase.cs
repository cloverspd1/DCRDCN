namespace BEL.DataAccessLayer
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.IO;
    using System.Linq;
    using System.Runtime.Caching;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Caching Provider Base Class
    /// </summary>
    public abstract class CachingProviderBase : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CachingProviderBase"/> class.
        /// </summary>
        public CachingProviderBase()
        {
            this.DeleteLog();
        }

        /// <summary>
        /// The cache
        /// </summary>
        private MemoryCache cache = new MemoryCache("CachingProvider");  ////MemoryCache.Default; ////

        /// <summary>
        /// The padlock
        /// </summary>
        private static readonly object Padlock = new object();

        /// <summary>
        /// Gets the caching keys.
        /// </summary>
        /// <value>
        /// The caching keys.
        /// </value>
        private List<string> CachingNotRequiredFor
        {
            get
            {
                BELDataAccessLayer helper = new BELDataAccessLayer();
                string keys = helper.GetConfigVariable("CachingNotRequiredFor");
                return keys.Trim().Trim(',').Split(',').ToList();
            }
        }

        /// <summary>
        /// Gets the cache interval.
        /// </summary>
        /// <value>
        /// The cache interval.
        /// </value>
        private Dictionary<string, string> CacheInterval
        {
            get
            {
                BELDataAccessLayer helper = new BELDataAccessLayer();
                string keys = helper.GetConfigVariable("CacheInterval");
                Dictionary<string, string> cacheInterval = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(keys))
                {
                    foreach (string str in keys.Trim().Trim(',').Split(',').ToList())
                    {
                        cacheInterval.Add(str.Trim().Split('|')[0].Trim(), str.Trim().Split('|')[1].Trim());
                    }
                }

                return cacheInterval;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [cache mode].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [cache mode]; otherwise, <c>false</c>.
        /// </value>
        private bool CacheMode
        {
            get
            {
                BELDataAccessLayer helper = new BELDataAccessLayer();
                return Convert.ToBoolean(helper.GetConfigVariable("CacheMode"));
            }
        }

        /// <summary>
        /// Adds the item.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        protected virtual void AddItem(string key, object value)
        {
            lock (Padlock)
            {
                if (this.CacheMode && !this.CachingNotRequiredFor.Contains(key))
                {
                    CacheItemPolicy policy = new CacheItemPolicy();
                    if (this.CacheInterval.ContainsKey(key))
                    {
                        policy.AbsoluteExpiration = DateTimeOffset.Now.AddHours(Convert.ToDouble(this.CacheInterval[key]));
                    }
                    else
                    {
                        policy.AbsoluteExpiration = DateTimeOffset.MaxValue;
                    }

                    this.cache.Add(key, value, policy);
                }
            }
        }

        /// <summary>
        /// Removes the item.
        /// </summary>
        /// <param name="key">The key.</param>
        protected virtual void RemoveItem(string key)
        {
            lock (Padlock)
            {
                this.cache.Remove(key);
            }
        }

        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="remove">if set to <c>true</c> [remove].</param>
        /// <returns>item object</returns>
        protected virtual object GetItem(string key, bool remove)
        {
            lock (Padlock)
            {
                var res = this.cache[key];

                if (res != null)
                {
                    if (remove == true)
                    {
                        this.cache.Remove(key);
                    }
                }
                else
                {
                    this.WriteToLog("CachingProvider-GetItem: Don't contains key: " + key);
                }
                var resCopy = DeepCopy(res);
                return resCopy;
            }
        }

        /// <summary>
        /// Gets all keys.
        /// </summary>
        /// <returns>list of keys</returns>
        protected virtual List<string> GetAllKeys()
        {
            List<string> list = new List<string>();
            foreach (var a in this.cache.ToList())
            {
                list.Add(a.Key);
            }

            return list;
        }

        #region Error Logs

        /// <summary>
        /// The log path
        /// </summary>
        private string logPath = System.Environment.GetEnvironmentVariable("TEMP");

        /// <summary>
        /// Deletes the log.
        /// </summary>
        protected void DeleteLog()
        {
            System.IO.File.Delete(string.Format("{0}\\CachingProvider_Errors.txt", this.logPath));
        }

        /// <summary>
        /// Writes to log.
        /// </summary>
        /// <param name="text">The text.</param>
        protected void WriteToLog(string text)
        {
            using (System.IO.TextWriter tw = System.IO.File.AppendText(string.Format("{0}\\CachingProvider_Errors.txt", this.logPath)))
            {
                tw.WriteLine(text);
                tw.Close();
            }
        }
        #endregion

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="isDisposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                this.cache.Dispose();
            }
        }

        /// <summary>
        /// Deeps the copy.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>object value</returns>
        public static object DeepCopy(object obj)
        {
            if (obj != null)
            {
                using (var ms = new MemoryStream())
                {
                    var formatter = new BinaryFormatter();
                    formatter.Serialize(ms, obj);
                    ms.Position = 0;
                    return formatter.Deserialize(ms);
                }
            }
            else
            {
                return null;
            }
        }
    }
}
