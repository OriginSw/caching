using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sixeyed.Caching.AOP;
using Sixeyed.Caching.Caches;
using Sixeyed.Caching.Tests.Stubs;

namespace Sixeyed.Caching.Tests.Caching
{
    [TestClass]
    public class MultiLevelCacheTests
    {
        private ICache _cache;
        private ICache _cacheL1 = Cache.AspNet;
        private ICache _cacheL2 = Cache.Memory;

        private const int _cacheL1ExpirationMsecs = 500;

        public MultiLevelCacheTests()
        {
            _cache = new MultiLevelCache(
                level1: _cacheL1,
                level2: _cacheL2,
                level1Expiration: TimeSpan.FromMilliseconds(_cacheL1ExpirationMsecs));
        }

        [TestMethod]
        [ExpectedException(typeof(CacheKeyNotFoundException))]
        public void Get_CacheKeyNotFoundException()
        {
            var key = Guid.NewGuid().ToString();
            _cache.Get<StubRequest>(key);
        }

        [TestMethod]
        public void Set()
        {
            var key = Guid.NewGuid().ToString();
            var value = StubRequest.GetRequest();

            _cache.Set(key, value, TimeSpan.FromSeconds(2));

            Assert.IsTrue(_cache.Exists(key));
            Assert.IsTrue(_cacheL1.Exists(key));
            Assert.IsTrue(_cacheL2.Exists(key));
            var retrievedValue1 = _cache.Get<StubRequest>(key);

            Thread.Sleep(_cacheL1ExpirationMsecs);

            Assert.IsTrue(_cache.Exists(key));
            Assert.IsFalse(_cacheL1.Exists(key));
            Assert.IsTrue(_cacheL2.Exists(key));
            var retrievedValue2 = _cache.Get<StubRequest>(key);

            Assert.AreEqual(value.CreatedOn, retrievedValue1.CreatedOn);
            Assert.AreEqual(value.Id, retrievedValue1.Id);
            Assert.AreEqual(value.Name, retrievedValue1.Name);

            Assert.AreEqual(value.CreatedOn, retrievedValue2.CreatedOn);
            Assert.AreEqual(value.Id, retrievedValue2.Id);
            Assert.AreEqual(value.Name, retrievedValue2.Name);
        }
    }
}
