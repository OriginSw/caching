
using System;
namespace Sixeyed.Caching
{
    public class CacheKey
    {
        public string Key { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}
