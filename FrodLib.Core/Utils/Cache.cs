﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

#if !USE_FROD_READ_WRITE_LOCK_IMPLEMENTATION
using FrodReaderWriterLock = System.Threading.ReaderWriterLockSlim;
using LockRecursionPolicy = System.Threading.LockRecursionPolicy;
#endif

namespace FrodLib.Utils
{
    public class CachedItemRemovedEventArgs<T> : EventArgs
    {
        public CachedItemRemovedEventArgs(string key, T removedObject)
        {
            this.Key = key;
            RemovedObject = removedObject;
        }

        public string Key { get; private set; }
        public T RemovedObject { get; private set; }
    }

    /// <summary>
	/// This is a generic cache subsystem based on key/value pairs.
	/// You can add any item to this cache as long as the key is unique, so treat keys as something like namespaces and build them with a 
	/// specific system/syntax in your application.
	/// Every cache entry has its own timeout.
	/// Cache is thread safe and will delete expired entries on its own using System.Threading.Timers (which run on <see cref="ThreadPool"/> threads).
	/// </summary>
    public class Cache<T> : IDisposable
    {
        public event EventHandler<CachedItemRemovedEventArgs<T>> CachedItemRemoved;

        #region Constructor and class members
        /// <summary>
        /// Initializes a new instance of the <see cref="Cache{T}"/> class.
        /// </summary>
        public Cache() { }

        private Dictionary<string, T> cache = new Dictionary<string, T>();
        private Dictionary<string, Timer> timers = new Dictionary<string, Timer>();
        private FrodReaderWriterLock locker = new FrodReaderWriterLock();
        #endregion

        #region IDisposable implementation
        private bool disposed = false;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///   <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                disposed = true;

                if (disposing)
                {
                    // Dispose managed resources.
                    locker.EnterWriteLock();
                    try
                    {
                        foreach (Timer t in timers.Values)
                            try { t.Dispose(); }
                            catch
                            { }

                        timers.Clear();
                        cache.Clear();
                    }
                    finally { locker.ExitWriteLock(); }

                    locker.Dispose();
                }
                // Dispose unmanaged resources
            }
        }
        #endregion

        #region CheckTimer
        // Checks whether a specific timer already exists and adds a new one, if not 
        private void CheckTimer(string key, int cacheTimeout, bool restartTimerIfExists)
        {
            Timer timer;

            if (timers.TryGetValue(key, out timer))
            {
                if (restartTimerIfExists)
                {
                    timer.Change(
                        (cacheTimeout == Timeout.Infinite ? Timeout.Infinite : cacheTimeout * 1000),
                        Timeout.Infinite);
                }
            }
            else
                timers.Add(
                    key,
                    new Timer(
                        RemoveByTimer,
                        key,
                        (cacheTimeout == Timeout.Infinite ? Timeout.Infinite : cacheTimeout * 1000),
                        Timeout.Infinite));
        }

        private void RemoveByTimer(object state)
        {
            Remove(state.ToString());
        }
        #endregion

        #region AddOrUpdate, Get, Remove, Exists 
        /// <summary>
        /// Adds or updates the specified cache-key with the specified cacheObject and applies a specified timeout (in seconds) to this key.
        /// </summary>
        /// <param name="key">The cache-key to add or update.</param>
        /// <param name="cacheObject">The cache object to store.</param>
        /// <param name="cacheTimeout">The cache timeout (lifespan) of this object. Must be 1 or greater.
        /// Specify Timeout.Infinite to keep the entry forever.</param>
        /// <param name="restartTimerIfExists">(Optional). If set to <c>true</c>, the timer for this cacheObject will be reset if the object already
        /// exists in the cache. (Default = false).</param>
        public void AddOrUpdate(string key, T cacheObject, int cacheTimeout, bool restartTimerIfExists = false)
        {
            if (disposed) return;

            if (cacheTimeout != Timeout.Infinite && cacheTimeout < 1)
                throw new ArgumentOutOfRangeException("cacheTimeout must be greater than zero or Timeout.Infinite.");

            locker.EnterWriteLock();
            try
            {
                CheckTimer(key, cacheTimeout, restartTimerIfExists);

                if (!cache.ContainsKey(key))
                    cache.Add(key, cacheObject);
                else
                    cache[key] = cacheObject;
            }
            finally { locker.ExitWriteLock(); }
        }

        /// <summary>
        /// Gets the cache entry with the specified key or returns <c>default(T)</c> if the key is not found.
        /// </summary>
        /// <param name="key">The cache-key to retrieve.</param>
        /// <returns>The object from the cache or <c>default(T)</c>, if not found.</returns>
        public T this[string key] => Get(key);

        /// <summary>
        /// Gets the cache entry with the specified key or return <c>default(T)</c> if the key is not found.
        /// </summary>
        /// <param name="key">The cache-key to retrieve.</param>
        /// <returns>The object from the cache or <c>default(T)</c>, if not found.</returns>
        public T Get(string key)
        {
            if (disposed) return default(T);

            locker.EnterReadLock();
            try
            {
                T rv;
                return (cache.TryGetValue(key, out rv) ? rv : default(T));
            }
            finally { locker.ExitReadLock(); }
        }

        /// <summary>
        /// Tries to gets the cache entry with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">(out) The value, if found, or <c>default(T)</c>, if not.</param>
        /// <returns><c>True</c>, if <c>key</c> exists, otherwise <c>false</c>.</returns>
        public bool TryGet(string key, out T value)
        {
            if (disposed)
            {
                value = default(T);
                return false;
            }

            locker.EnterReadLock();
            try
            {
                return cache.TryGetValue(key, out value);
            }
            finally { locker.ExitReadLock(); }
        }

        /// <summary>
        /// Removes the specified cache entry with the specified key.
        /// If the key is not found, no exception is thrown, the statement is just ignored.
        /// </summary>
        /// <param name="key">The cache-key to remove.</param>
        public bool Remove(string key)
        {
            if (disposed) return false;

            bool removed = false;
            locker.EnterWriteLock();
            try
            {
                T objToBeremoved;
                if (cache.TryGetValue(key, out objToBeremoved))
                {
                    try { timers[key].Dispose(); }
                    catch { }
                    timers.Remove(key);
                    removed = cache.Remove(key);
                    OnItemRemoved(key, objToBeremoved);
                }
            }
            finally { locker.ExitWriteLock(); }
            return removed;
        }

        private void OnItemRemoved(string key, T objToBeremoved)
        {
            if (objToBeremoved == null) return;
            var handler = CachedItemRemoved;
            if (handler != null)
            {
                handler(this, new CachedItemRemovedEventArgs<T>(key, objToBeremoved));
            }
        }

        /// <summary>
        /// Checks if a specified key exists in the cache.
        /// </summary>
        /// <param name="key">The cache-key to check.</param>
        /// <returns><c>True</c> if the key exists in the cache, otherwise <c>False</c>.</returns>
        public bool Exists(string key)
        {
            if (disposed) return false;

            locker.EnterReadLock();
            try
            {
                return cache.ContainsKey(key);
            }
            finally { locker.ExitReadLock(); }
        }
        #endregion

        public string[] GetKeys()
        {
            return cache.Keys.OfType<string>().ToArray();
        }
    }

    #region Cache class (non-generic)
    /// <summary>
    /// The non-generic Cache class instanciates a Cache{object} that can be used with any type of (mixed) contents.
    /// It also publishes a static <c>.Global</c> member, so a cache can be used even without creating a dedicated instance.
    /// The <c>.Global</c> member is lazy instanciated.
    /// </summary>
    public class Cache : Cache<object>
    {
        #region Static Global Cache instance 
        private static Lazy<Cache> global = new Lazy<Cache>();
        /// <summary>
        /// Gets the global shared cache instance valid for the entire process.
        /// </summary>
        /// <value>
        /// The global shared cache instance.
        /// </value>
        public static Cache Global => global.Value;
        #endregion
    }
    #endregion
}
