using Sixeyed.Caching.Configuration;
using Sixeyed.Caching.Extensions;
using Sixeyed.Caching.Logging;
using Sixeyed.Caching.Serialization;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Sixeyed.Caching
{
    /// <summary>
    /// Class for building cache keys based on method call signatures
    /// </summary>
    public static class CacheKeyBuilder
    {
        /// <summary>
        /// Builds a full cache key using the provided format
        /// </summary>
        /// <remarks>
        /// Returns a hashed GUID of the input, so any size input will be a 16-character key
        /// </remarks>
        /// <param name="keyFormat"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string GetCacheKey(string keyFormat, params object[] args)
        {
            var cacheKey = keyFormat.FormatWith(args);
            return HashCacheKey(cacheKey);
        }

        public static string HashCacheKey(string cacheKey)
        {
            //hash the string as a GUID:
            byte[] hashBytes;
            using (var provider = new MD5CryptoServiceProvider())
            {
                var inputBytes = Encoding.Default.GetBytes(cacheKey);
                hashBytes = provider.ComputeHash(inputBytes);
            }
            return new Guid(hashBytes).ToString();
        }
    }
}
