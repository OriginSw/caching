using System;
using System.Collections.Generic;
using System.Linq;
using Sixeyed.Caching.Configuration;
using Sixeyed.Caching.Logging;
using Sixeyed.Caching.Serialization;
using sys = System.Runtime.Caching;

namespace Sixeyed.Caching.Caches
{
    /// <summary>
    /// <see cref="ITrackeableCache"/> implementation that wraps a <see cref="ICache"/> and tracks the inserted keys
    /// </summary>
    public class TrackeableCache : ITrackeableCache
    {
        private const string _DEFAULT_KEY = "CACHED_KEYS";

        private string _key;
        private ICache _cache;
        private ICache _keysCache;

        /// <summary>
        /// Returns the cache type
        /// </summary>
        public CacheType CacheType
        {
            get { return _cache.CacheType; }
        }

        public TrackeableCache(string key, ICache cache, ICache keysCache = null)
            : base()
        {
            _key = key;
            _cache = cache;
            _keysCache = keysCache ?? cache;
        }

        protected virtual IList<CacheKey> GetCacheKeys()
        {
            try
            {
                return _keysCache.Get<IList<CacheKey>>(_key)
                    .Where(x => !IsExpired(x.ExpiresAt))
                    .ToList();
            }
            catch (CacheKeyNotFoundException)
            {
                return new List<CacheKey>();
            }
        }

        protected virtual void SaveKeys(IList<CacheKey> keys)
        {
            _keysCache.Set(_key, keys);
        }

        protected virtual void AddKey(string key, DateTime? expiresAt)
        {
            var keys = GetCacheKeys();

            var existingKey = keys.FirstOrDefault(x => x.Key == key);
            if(existingKey == null)
            {
                if(IsExpired(expiresAt))
                    return;
                keys.Add(new CacheKey() { Key = key, ExpiresAt = expiresAt });
            }
            else
            {
                if(IsExpired(expiresAt))
                    keys.Remove(existingKey);
                else
                    existingKey.ExpiresAt = expiresAt;
            }
            SaveKeys(keys);
        }

        protected virtual void RemoveKey(string key)
        {
            var keys = GetCacheKeys();

            var existingKey = keys.FirstOrDefault(x => x.Key == key);
            if (existingKey == null)
                return;

            keys.Remove(existingKey);
            SaveKeys(keys);
        }

        protected virtual bool IsExpired(DateTime? expiresAt)
        {
            return expiresAt != null && expiresAt <= DateTime.UtcNow;
        }

        public IEnumerable<string> GetKeys()
        {
            return GetCacheKeys()
                .Select(x => x.Key)
                .ToList();
        }

        public void RemoveAll(string keyPrefix = null)
        {
            GetCacheKeys()
                .Where(x => keyPrefix == null || x.Key.StartsWith(keyPrefix))
                .ToList()
                .ForEach(x => Remove(x.Key));
        }


        public Serialization.Serializer Serializer
        {
            get { return _cache.Serializer; }
            set { _cache.Serializer = value; }
        }

        public void Initialise()
        {
            _cache.Initialise();
            if(_cache != _keysCache)
                _keysCache.Initialise();
        }

        public void Set(string key, object value, Serialization.SerializationFormat serializationFormat = SerializationFormat.Null)
        {
            _cache.Set(key, value, serializationFormat);
            AddKey(key, expiresAt: null);
        }

        public void Set(string key, object value, DateTime expiresAt, Serialization.SerializationFormat serializationFormat = SerializationFormat.Null)
        {
            _cache.Set(key, value, expiresAt, serializationFormat);
            AddKey(key, expiresAt: expiresAt);
        }

        public void Set(string key, object value, TimeSpan validFor, Serialization.SerializationFormat serializationFormat = SerializationFormat.Null)
        {
            _cache.Set(key, value, validFor, serializationFormat);
            AddKey(key, expiresAt: DateTime.UtcNow.Add(validFor));
        }

        public object Get(Type type, string key, Serialization.SerializationFormat serializationFormat = SerializationFormat.Null)
        {
            return _cache.Get(type, key, serializationFormat);
        }

        public T Get<T>(string key, Serialization.SerializationFormat serializationFormat = SerializationFormat.Null)
        {
            return _cache.Get<T>(key, serializationFormat);
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        public bool Exists(string key)
        {
            return _cache.Exists(key);
        }
    }
}
