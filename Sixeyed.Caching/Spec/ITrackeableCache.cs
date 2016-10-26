using Sixeyed.Caching.Serialization;
using System;
using System.Collections.Generic;

namespace Sixeyed.Caching
{
    public interface ITrackeableCache : ICache
    {
        /// <summary>
        /// Retrieves all keys
        /// </summary>
        /// <returns>Cached value or null</returns>
        IEnumerable<string> GetKeys();

        /// <summary>
        /// Removes all values
        /// </summary>
        /// <param name="keyPrefix">Optionally filters by key prefix</param>
        void RemoveAll(string keyPrefix = null);
    }
}
