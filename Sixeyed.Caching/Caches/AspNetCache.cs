using System;
using System.Collections.Generic;
using Sixeyed.Caching.Configuration;
using Sixeyed.Caching.Logging;
using sys = System.Web.Caching;

namespace Sixeyed.Caching.Caches
{
    /// <summary>
    /// <see cref="ICache"/> implementation using .NET AspNetCache as the backing cache
    /// </summary>
    /// <remarks>
    /// Uses CacheConfiguration setting "defaultCacheName" to determine the cache name.
    /// Defaults to "Sixeyed.Caching.Cache" if not set
    /// </remarks>
    public class AspNetCache : CacheBase
    {
        private sys.Cache _cache;

        /// <summary>
        /// Returns the cache type
        /// </summary>
        public override CacheType CacheType
        {
            get { return CacheType.AspNet; }
        }

        protected override void InitialiseInternal()
        {
            if (_cache == null)
            {
                Log.Debug("AspNetCache.Initialise - initialising with cacheName: {0}", CacheConfiguration.Current.DefaultCacheName);
                _cache = System.Web.HttpRuntime.Cache;
            }
        }

        protected override void SetInternal(string key, object value)
        {
            SetInternal(key, value, DateTime.UtcNow.AddYears(99));
        }

        /// <summary>
        /// Insert or update a cache value with a fixed lifetime
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="lifespan"></param>
        protected override void SetInternal(string key, object value, TimeSpan lifespan)
        {
            _cache.Insert(key, value, null, sys.Cache.NoAbsoluteExpiration, lifespan);
        }

        /// <summary>
        /// Insert or update a cache value with an expiry date
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresAt"></param>
        protected override void SetInternal(string key, object value, DateTime expiresAt)
        {
            _cache.Insert(key, value, null, expiresAt, sys.Cache.NoSlidingExpiration);
        }

        /// <summary>
        /// Retrieve a value from cache
        /// </summary>
        /// <param name="key"></param>
        /// <returns>Cached value or null</returns>
        protected override object GetInternal(string key)
        {
            return _cache[key];
        }

        protected override bool ExistsInternal(string key)
        {
            return _cache[key] != null;
        }

        protected override void RemoveInternal(string key)
        {
            _cache.Remove(key);
        }

        protected override List<string> GetAllKeys()
        {
            var keys = new List<string>();
            var enumerator = _cache.GetEnumerator();
            while (enumerator.MoveNext())
            {
                keys.Add(enumerator.Key.ToString());
            }
            return keys;
        }
    }
}
