using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sixeyed.Caching.Extensions;
using System.Linq;
using System.Threading;
using Sixeyed.Caching;
using Sixeyed.Caching.Tests.Stubs;
using Sixeyed.Caching.AOP;

namespace Sixeyed.Caching.Tests.Caching
{
    [TestClass]
    public class AspNetCacheTests
    {
        [TestMethod]
        [ExpectedException(typeof(CacheKeyNotFoundException))]
        public void Get_CacheKeyNotFoundException()
        {
            var key = Guid.NewGuid().ToString();
            Cache.AspNet.Get<StubRequest>(key);
        }

        [TestMethod]
        [ExpectedException(typeof(CacheValueCastException))]
        public void Get_Null_CacheValueCastException()
        {
            var key = Guid.NewGuid().ToString();
            int? value = null;
            Cache.AspNet.Set(key, value);
            var retrievedValue = Cache.AspNet.Get<int>(key);
        }

        [TestMethod]
        public void Set()
        {
            var key = Guid.NewGuid().ToString();
            var value = StubRequest.GetRequest();
            Cache.AspNet.Set(key, value);
            Assert.IsTrue(Cache.AspNet.Exists(key));
            var retrievedValue = Cache.AspNet.Get<StubRequest>(key);
            Assert.AreEqual(value.CreatedOn, retrievedValue.CreatedOn);
            Assert.AreEqual(value.Id, retrievedValue.Id);
            Assert.AreEqual(value.Name, retrievedValue.Name);
        }

        [TestMethod]
        public void Set_Null()
        {
            var key = Guid.NewGuid().ToString();
            StubRequest value = null;
            Assert.IsFalse(Cache.AspNet.Exists(key));
            Cache.AspNet.Set(key, value);
            Assert.IsTrue(Cache.AspNet.Exists(key));
            var retrievedValue = Cache.AspNet.Get<StubRequest>(key);
            Assert.IsNull(retrievedValue);
        }

        [TestMethod]
        public void Set_WithAbsoluteExpiry()
        {            
            var key = Guid.NewGuid().ToString();
            var value = StubRequest.GetRequest();
            var expiresAt = DateTime.Now.AddMilliseconds(250);
            Cache.AspNet.Set(key, value, expiresAt);
            Assert.IsTrue(Cache.AspNet.Exists(key));
            Thread.Sleep(500);
            Assert.IsFalse(Cache.AspNet.Exists(key));
        }

        [TestMethod]
        public void Set_WithSlidingExpiry()
        {
            var key = Guid.NewGuid().ToString();
            var value = StubRequest.GetRequest();
            var lifespan = new TimeSpan(0, 0, 0, 0, 250);
            Cache.AspNet.Set(key, value, lifespan);
            Assert.IsTrue(Cache.AspNet.Exists(key));
            Thread.Sleep(200);
            var retrieved = Cache.AspNet.Get<StubRequest>(key);
            Assert.IsNotNull(retrieved);
            
            Thread.Sleep(50);            
            Assert.IsFalse(Cache.AspNet.Exists(key));
        }

        [TestMethod]
        public void Set_ThenRemove()
        {
            var key = Guid.NewGuid().ToString();
            var value = StubRequest.GetRequest();
            Cache.AspNet.Set(key, value);
            Assert.IsTrue(Cache.AspNet.Exists(key));
            Cache.AspNet.Remove(key);
            Assert.IsFalse(Cache.AspNet.Exists(key));
        }

        [TestMethod]
        public void Set_ThenGetAll()
        {
            var key1 = Guid.NewGuid().ToString();
            var value1 = StubRequest.GetRequest();
            Cache.AspNet.Set(key1, value1);

            var key2 = Guid.NewGuid().ToString();
            var value2 = StubRequest.GetRequest();
            Cache.AspNet.Set(key2, value2);

            var all = Cache.AspNet.GetAll<StubRequest>();
            Assert.IsNotNull(all);
            Assert.AreEqual(2, all.Count());
            Assert.IsTrue(all.ContainsKey(key1));
            Assert.AreEqual(value1.Id, all[key1].Id);
            Assert.IsTrue(all.ContainsKey(key2));
            Assert.AreEqual(value2.Id, all[key2].Id);
        }

        [TestMethod]
        public void Set_ThenRemoveAll()
        {
            var key1 = Guid.NewGuid().ToString();
            var value1 = StubRequest.GetRequest();
            Cache.AspNet.Set(key1, value1);

            var key2 = Guid.NewGuid().ToString();
            var value2 = StubRequest.GetRequest();
            Cache.AspNet.Set(key2, value2);

            Assert.IsTrue(Cache.AspNet.Exists(key1));
            Assert.IsTrue(Cache.AspNet.Exists(key2));

            Cache.AspNet.RemoveAll();

            Assert.IsFalse(Cache.AspNet.Exists(key1));
            Assert.IsFalse(Cache.AspNet.Exists(key2));
        }

        [TestMethod]
        public void Set_ThenRemoveAllByKeyPrefix()
        {
            const string keyPrefix = "PREFIX_";

            var key1 = keyPrefix + Guid.NewGuid().ToString();
            var value1 = StubRequest.GetRequest();
            Cache.AspNet.Set(key1, value1);

            var key2 = Guid.NewGuid().ToString();
            var value2 = StubRequest.GetRequest();
            Cache.AspNet.Set(key2, value2);

            Assert.IsTrue(Cache.AspNet.Exists(key1));
            Assert.IsTrue(Cache.AspNet.Exists(key2));

            Cache.AspNet.RemoveAll(keyPrefix);

            Assert.IsFalse(Cache.AspNet.Exists(key1));
            Assert.IsTrue(Cache.AspNet.Exists(key2));
        }

        [TestMethod]
        public void RemoveUnexisting()
        {
            var key = Guid.NewGuid().ToString();
            Cache.AspNet.Remove(key);
        }
    }
}
