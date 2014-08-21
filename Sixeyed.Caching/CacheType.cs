
namespace Sixeyed.Caching
{
    public enum CacheType
    {
        /// <summary>
        /// No cache type set
        /// </summary>
        Null = 0,

        Memory,

        AspNet,

        AppFabric,

        Memcached,

        AzureTableStorage,

        Disk,

        Couchbase
    }
}
