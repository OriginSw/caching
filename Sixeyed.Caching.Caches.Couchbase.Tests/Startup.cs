using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sixeyed.Caching.AOP;
using Sixeyed.Caching.Serialization;
using Sixeyed.Caching.Serialization.Serializers.Json;
using Sixeyed.Caching.Serialization.Serializers;
using Sixeyed.Caching.Caches.Couchbase;

namespace Sixeyed.Caching.Tests
{
    [TestClass]
    public static class Startup
    {
        [AssemblyInitialize()]
        public static void Init(TestContext context)
        {
            JsonSerializer();
            Couchbase();
        }

        private static void JsonSerializer()
        {
            //var thisAssembly = typeof(JsonSerializer).Assembly;
            //Container.RegisterAll<IJsonSerializer>(thisAssembly, Lifetime.Singleton);
            Serializer.Default.Json = new JsonSerializer();
        }

        private static void Couchbase()
        {
            var thisAssembly = typeof(CouchbaseCache).Assembly;
            Container.RegisterAllCache<ICache>(thisAssembly, Lifetime.Singleton);
        }
    }
}
