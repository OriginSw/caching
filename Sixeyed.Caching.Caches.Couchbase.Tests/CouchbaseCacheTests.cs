using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sixeyed.Caching.AOP;
using Sixeyed.Caching.Caches.Couchbase.Tests.Stubs;

namespace Sixeyed.Caching.Caches.Couchbase.Tests
{
    [TestClass]
    public class CouchbaseCacheTests
    {
        private ICache _cache = Cache.Couchbase;

        [TestMethod]
        [ExpectedException(typeof(CacheKeyNotFoundException))]
        public void Get_CacheKeyNotFoundException()
        {
            var key = Guid.NewGuid().ToString();
            _cache.Get<StubRequest>(key);
        }

        [TestMethod]
        [ExpectedException(typeof(CacheValueCastException))]
        public void Get_Null_CacheValueCastException()
        {
            var key = Guid.NewGuid().ToString();
            int? value = null;
            _cache.Set(key, value);
            var retrievedValue = _cache.Get<int>(key);
        }

        [TestMethod]
        public void Set()
        {
            var key = Guid.NewGuid().ToString();
            var value = StubRequest.GetRequest();
            _cache.Set(key, value);
            Assert.IsTrue(_cache.Exists(key));
            var retrievedValue = _cache.Get<StubRequest>(key);
            Assert.AreEqual(value.CreatedOn, retrievedValue.CreatedOn);
            Assert.AreEqual(value.Id, retrievedValue.Id);
            Assert.AreEqual(value.Name, retrievedValue.Name);
        }

        [TestMethod]
        public void Set_Null()
        {
            var key = Guid.NewGuid().ToString();
            StubRequest value = null;
            Assert.IsFalse(_cache.Exists(key));
            _cache.Set(key, value);
            Assert.IsTrue(_cache.Exists(key));
            var retrievedValue = _cache.Get<StubRequest>(key);
            Assert.IsNull(retrievedValue);
        }

        [TestMethod]
        public void Set_WithAbsoluteExpiry()
        {
            var key = Guid.NewGuid().ToString();
            var value = StubRequest.GetRequest();
            var expiresAt = DateTime.Now.AddMilliseconds(1250);
            _cache.Set(key, value, expiresAt);
            Assert.IsTrue(_cache.Exists(key));
            Thread.Sleep(3000);
            Assert.IsFalse(_cache.Exists(key));
        }

        [TestMethod]
        [ExpectedException(typeof(CacheKeyNotFoundException))]
        public void Set_WithTimeoutExpiry()
        {
            var key = Guid.NewGuid().ToString();
            var value = StubRequest.GetRequest();
            var lifespan = new TimeSpan(0, 0, 0, 1, 250);
            _cache.Set(key, value, lifespan);
            Assert.IsTrue(_cache.Exists(key));
            Thread.Sleep(200);
            var retrieved = _cache.Get<StubRequest>(key);
            Assert.IsNotNull(retrieved);
            
            Thread.Sleep(2000);
            Assert.IsFalse(_cache.Exists(key));
            retrieved = _cache.Get<StubRequest>(key);
        }

        [TestMethod]
        public void Set_ThenRemove()
        {
            var key = Guid.NewGuid().ToString();
            var value = StubRequest.GetRequest();
            _cache.Set(key, value);
            Assert.IsTrue(_cache.Exists(key));
            _cache.Remove(key);
            Assert.IsFalse(_cache.Exists(key));
        }
    }
}
