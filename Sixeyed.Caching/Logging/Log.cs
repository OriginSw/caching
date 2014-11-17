using Sixeyed.Caching.Extensions;
using System;
using System.Collections.Generic;
using Bardock.Utils.Logger;

namespace Sixeyed.Caching.Logging
{
    /// <summary>
    /// Log wrapper
    /// </summary>
    public class Log
    {
        private static ILog GetLogger(string loggerName = null)
        {
            if (loggerName.IsNullOrEmpty())
            {
                loggerName = LoggerName.Default;
            }
            return LogManager.Default.GetLog(loggerName);
        }

        public static string GetDefaultLoggerName()
        {
            return LoggerName.Default;
        }

        /// <summary>
        /// Gets a named logger to write to
        /// </summary>
        /// <param name="loggerName"></param>
        /// <returns></returns>
        public static ILog Using(string loggerName)
        {
            return GetLogger(loggerName);
        }

        /// <summary>
        /// Formats and writes a DEBUG-level message to the log, using the log4net configuration
        /// </summary>
        /// <param name="message">Log message format</param>
        /// <param name="args">Log message arguments</param>
        public static void Debug(string message, params object[] args)
        {
            GetLogger().Debug(string.Format(message, args));
        }

        /// <summary>
        /// Formats and writes an INFO-level message to the log, using the log4net configuration
        /// </summary>
        /// <param name="message">Log message format</param>
        /// <param name="args">Log message arguments</param>
        public static void Info(string message, params object[] args)
        {
            GetLogger().Info(string.Format(message, args));
        }

        /// <summary>
        /// Formats and writes a WARN-level message to the log, using the log4net configuration
        /// </summary>
        /// <param name="message">Log message format</param>
        /// <param name="args">Log message arguments</param>
        public static void Warn(string message, params object[] args)
        {
            GetLogger().Warn(string.Format(message, args));
        }

        /// <summary>
        /// Formats and writes a ERROR-level message to the log, using the log4net configuration
        /// </summary>
        /// <param name="message">Log message format</param>
        /// <param name="args">Log message arguments</param>
        public static void Error(string message, params object[] args)
        {
            GetLogger().Error(string.Format(message, args));
        }
        
        /// <summary>
        /// Formats and writes a ERROR-level message to the log, using the log4net configuration, appending exception details
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <param name="message">Log message format</param>
        /// <param name="args">Log message arguments</param>
        public static void Error(Exception ex, string message, params object[] args)
        {
            GetLogger().Error(string.Format(message, args), ex);
        }
        /// <summary>
        /// Formats and writes a FATAL-level message to the log, using the log4net configuration
        /// </summary>
        /// <param name="message">Log message format</param>
        /// <param name="args">Log message arguments</param>
        public static void Fatal(string message, params object[] args)
        {
            GetLogger().Fatal(string.Format(message, args));
        }

        private struct LoggerName
        {
            public const string Default = "Sixeyed.Caching";
        }
    }
}

