using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sixeyed.Caching.Caches.AppFabric;
using Sixeyed.Caching.Caches.AzureStorage;
using Sixeyed.Caching.Caches.Memcached;
using Sixeyed.Caching.AOP;
using Sixeyed.Caching.Serialization;
using Sixeyed.Caching.Serialization.Serializers.Json;
using Sixeyed.Caching.Serialization.Serializers;

namespace Sixeyed.Caching.Tests
{
    [TestClass]
    public static class Startup
    {
        [AssemblyInitialize()]
        public static void Init(TestContext context)
        {
            JsonSerializer();
            AppFabricCache();
            AzureTableStorageCache();
            Memcached();
        }

        private static void JsonSerializer()
        {
            //var thisAssembly = typeof(JsonSerializer).Assembly;
            //Container.RegisterAll<IJsonSerializer>(thisAssembly, Lifetime.Singleton);
            Serializer.Default.Json = new JsonSerializer();
        }

        private static void AppFabricCache()
        {
            var thisAssembly = typeof(AppFabricCache).Assembly;
            Container.RegisterAllCache<ICache>(thisAssembly, Lifetime.Singleton);
        }

        private static void AzureTableStorageCache()
        {
            var thisAssembly = typeof(AzureTableStorageCache).Assembly;
            Container.RegisterAllCache<ICache>(thisAssembly, Lifetime.Singleton);
        }

        private static void Memcached()
        {
            var thisAssembly = typeof(MemcachedCache).Assembly;
            Container.RegisterAllCache<ICache>(thisAssembly, Lifetime.Singleton);
        }
    }
}
