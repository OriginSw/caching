using System;
using System.Collections.Generic;
using System.Linq;
using Couchbase;
using Enyim.Caching.Memcached;
using Sixeyed.Caching.Logging;

namespace Sixeyed.Caching.Caches.Couchbase
{
    public class CouchbaseCache : CacheBase
    {
        public CouchbaseClient Client { get; protected set; }

        public override CacheType CacheType
        {
            get { return CacheType.Couchbase; }
        }

        protected override bool ItemsNeedSerializing
        {
            get { return true; }
        }

        protected override void InitialiseInternal()
        {
            if (Client == null)
            {
                Log.Debug("CouchbaseCache.Initialise - initialising");
                Client = new CouchbaseClient();
            }
        }

        protected override void SetInternal(string key, object value)
        {
            Client.Store(StoreMode.Set, key, value);
        }

        protected override void SetInternal(string key, object value, DateTime expiresAt)
        {
            Client.Store(StoreMode.Set, key, value, expiresAt);
        }

        protected override void SetInternal(string key, object value, TimeSpan validFor)
        {
            Client.Store(StoreMode.Set, key, value, validFor);
        }

        protected override object GetInternal(string key)
        {
            return Client.Get(key);
        }

        protected override void RemoveInternal(string key)
        {
            if (Exists(key))
            {
                Client.Remove(key);
            }
        }

        protected override bool ExistsInternal(string key)
        {
            return GetInternal(key) != null;
        }

        protected override List<string> GetAllKeys()
        {
            return this.Client.GetView(
                    designName: Configuration.CacheConfiguration.Current.AllKeysDesign,
                    viewName: Configuration.CacheConfiguration.Current.AllKeysView,
                    urlEncode: true)
                .Select(x => x.ItemId)
                .ToList();
        }
    }
}