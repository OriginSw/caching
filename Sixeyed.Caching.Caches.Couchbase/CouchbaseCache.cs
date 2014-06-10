using System;
using Couchbase;
using Enyim.Caching.Memcached;
using Sixeyed.Caching.Logging;

namespace Sixeyed.Caching.Caches.Couchbase
{
    public class CouchbaseCache : CacheBase
    {
        private CouchbaseClient _cache;

        public override CacheType CacheType
        {
            get { return CacheType.Couchbase; }
        }

        protected override void InitialiseInternal()
        {
            if (_cache == null)
            {
                Log.Debug("CouchbaseCache.Initialise - initialising");
                _cache = new CouchbaseClient();
            }
        }

        protected override void SetInternal(string key, object value)
        {
            _cache.Store(StoreMode.Set, key, value);
        }

        protected override void SetInternal(string key, object value, DateTime expiresAt)
        {
            _cache.Store(StoreMode.Set, key, value, expiresAt);
        }

        protected override void SetInternal(string key, object value, TimeSpan validFor)
        {
            _cache.Store(StoreMode.Set, key, value, validFor);
        }

        protected override object GetInternal(string key)
        {
            return _cache.Get(key);
        }

        protected override void RemoveInternal(string key)
        {
            if (Exists(key))
            {
                _cache.Remove(key);
            }
        }

        protected override bool ExistsInternal(string key)
        {
            return GetInternal(key) != null;
        }
    }
}
