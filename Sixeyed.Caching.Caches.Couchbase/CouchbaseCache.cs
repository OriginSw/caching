using Couchbase;
using Couchbase.Configuration.Client;
using Couchbase.Configuration.Client.Providers;
using Couchbase.Core;
using Sixeyed.Caching.Caches.Couchbase.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Sixeyed.Caching.Caches.Couchbase
{
    public class CouchbaseCache : CacheBase, IDisposable
    {
        public BucketConfiguration BucketConfig { get; protected set; }

        private ClientConfiguration _clientConfig;

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
            if (_clientConfig == null)
            {
                _clientConfig = _clientConfig ?? new ClientConfiguration((CouchbaseClientSection)ConfigurationManager.GetSection("couchbaseClients/couchbase"));
                BucketConfig = _clientConfig.BucketConfigs.Values.FirstOrDefault();
            }
            try
            {
                ClusterHelper.Get();
            }
            catch (InitializationException)
            {
                ClusterHelper.Initialize(_clientConfig);
            }
        }

        protected override void SetInternal(string key, object value)
        {
            var bucket = ClusterHelper.GetBucket(BucketConfig.BucketName, BucketConfig.Password);
            bucket.Upsert(key, value);
        }

        protected override void SetInternal(string key, object value, DateTime expiresAt)
        {
            var validFor = expiresAt - DateTime.Now;
            var bucket = ClusterHelper.GetBucket(BucketConfig.BucketName, BucketConfig.Password);
            bucket.Upsert(key, value, validFor);
        }

        protected override void SetInternal(string key, object value, TimeSpan validFor)
        {
            var bucket = ClusterHelper.GetBucket(BucketConfig.BucketName, BucketConfig.Password);
            bucket.Upsert(key, value, validFor);
        }

        protected override object GetInternal(string key)
        {
            var bucket = ClusterHelper.GetBucket(BucketConfig.BucketName, BucketConfig.Password);
            var doc = bucket.GetDocument<object>(key);
            return doc.Content;
        }

        protected override void RemoveInternal(string key)
        {
            var bucket = ClusterHelper.GetBucket(BucketConfig.BucketName, BucketConfig.Password);
            bucket.Remove(key);
        }

        protected override bool ExistsInternal(string key)
        {
            return GetInternal(key) != null;
        }

        protected override List<string> GetAllKeys()
        {
            var bucket = ClusterHelper.GetBucket(BucketConfig.BucketName, BucketConfig.Password);
            var query = bucket.CreateQuery(
                CacheConfiguration.Current.AllKeysDesign,
                CacheConfiguration.Current.AllKeysView,
                CacheConfiguration.Current.DevMode);

            var result = bucket.Query<Dictionary<string, object>>(query);

            var allKeys = result.Rows.Select(x => x.Id).ToList();

            return allKeys;
        }

        public void Dispose()
        {
            ClusterHelper.Close();
        }
    }
}