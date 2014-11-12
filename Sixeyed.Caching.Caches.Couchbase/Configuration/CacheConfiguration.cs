using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Sixeyed.Caching.Logging;

namespace Sixeyed.Caching.Caches.Couchbase.Configuration
{
    /// <summary>
    /// Configuration section for configuring Couchbase caching
    /// </summary>
    public class CacheConfiguration : ConfigurationSection
    {
        private static bool _loggedWarning;

        /// <summary>
        /// Returns the currently configured settings
        /// </summary>
        public static CacheConfiguration Current
        {
            get
            {
                var current = ConfigurationManager.GetSection("sixeyed.caching.caches.couchbase") as CacheConfiguration;
                if (current == null)
                {
                    current = new CacheConfiguration();
                    if (!_loggedWarning)
                    {
                        Log.Warn("Configuration section: <sixeyed.caching.caches.couchbase> not specified. Default configuration will be used");
                        _loggedWarning = true;
                    }
                }
                return current;
            }
        }

        /// <summary>
        /// Returns the design document name of the view that retrieves all cached keys
        /// </summary>
        [ConfigurationProperty(SettingName.AllKeysDesign)]
        public string AllKeysDesign
        {
            get { return (string)this[SettingName.AllKeysDesign]; }
        }

        /// <summary>
        /// Returns the name of the view that retrieves all cached keys
        /// </summary>
        [ConfigurationProperty(SettingName.AllKeysView)]
        public string AllKeysView
        {
            get { return (string)this[SettingName.AllKeysView]; }
        }

        /// <summary>
        /// Constants for indexing settings
        /// </summary>
        private struct SettingName
        {
            public const string AllKeysDesign = "allKeysDesign";
            public const string AllKeysView = "allKeysView";
        }
    }
}
