using Microsoft.Practices.Unity.InterceptionExtension;
using Sixeyed.Caching.Configuration;
using Sixeyed.Caching.AOP.Extensions;
using Sixeyed.Caching.AOP.Interception.Cache;
using Sixeyed.Caching.Extensions;
using Sixeyed.Caching.Logging;
using Sixeyed.Caching.Serialization;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Sixeyed.Caching.AOP
{
    /// <summary>
    /// Class for building cache keys based on method call signatures
    /// </summary>
    public static class CacheKeyBuilder
    {
        /// <summary>
        /// Builds a full cache key in the format:
        ///  ClassName_MethodName_argumentValue1_argumentValue2...._argumentValueN
        /// </summary>
        /// <remarks>
        /// Returns a hashed GUID of the input, so any size input will be a 16-characetr key
        /// </remarks>
        /// <param name="input"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public static string GetCacheKey(IMethodInvocation input, ISerializer serializer)
        {
            var prefix = GetCacheKeyPrefix(input);
            var key = input.ToTraceString(serializer, prefix);
            var hashedKey = Sixeyed.Caching.CacheKeyBuilder.HashCacheKey(key);
            if (!CacheConfiguration.Current.HashPrefixInCacheKey)
            {
                hashedKey = string.Format("{0}_{1}", prefix, hashedKey);
            }
            Log.Debug("CacheKeyBuilder.GetCacheKey - returned {0}", hashedKey);
            return hashedKey;
        }

        /// <summary>
        /// Returns the prefix to be used in a full cache key
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetCacheKeyPrefix(IMethodInvocation input)
        {
            var provider = input.Target as ICacheKeyPrefixProvider;
            var prefix = provider != null ? provider.GetCacheKeyPrefix() : input.GetMethodCallPrefix().Trim();
            Log.Debug("CacheKeyBuilder.GetCacheKeyPrefix - returned {0}", prefix);
            return prefix;
        }
    }
}
