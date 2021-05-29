namespace Caliburn.Noesis
{
    using System;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Logging;
    using ILogger = Microsoft.Extensions.Logging.ILogger;
#if UNITY_5_5_OR_NEWER
    using UnityEngine;
    using Object = UnityEngine.Object;

#endif

    /// <summary>
    ///     Responsible for creating various <see cref="Microsoft.Extensions.Logging.ILogger" />
    ///     types.
    /// </summary>
    [PublicAPI]
    public static class LogManager
    {
        #region Public Properties

        /// <summary>Gets the minimum <see cref="LogLevel" /> handled by generated loggers.</summary>
        public static LogLevel MinimumLogLevel { get; set; } = LogLevel.Debug;

        #endregion

        #region Public Methods

        /// <summary>Creates a named <see cref="ILogger" /> for the specified context.</summary>
        /// <param name="name">The category name of the logger.</param>
        /// <param name="context">
        ///     (Optional) The context of the logger. This should be a game object or a type
        ///     for non unity classes.
        /// </param>
        /// <returns>The logger for the specified context.</returns>
        public static ILogger CreateLogger(string name, object context = default)
        {
            if (UnitTestDetector.IsInUnitTest)
            {
                return CreateForTesting(name, context);
            }

#if UNITY_5_5_OR_NEWER
            return Application.isEditor
                       ? CreateForEditor(name, context)
                       : CreateForRuntime(name, context);
#else
            return CreateForEditor(name, context);
#endif
        }

        #endregion

        #region Private Methods

        private static ILogger CreateForEditor(string name, object context)
        {
#if UNITY_5_5_OR_NEWER
            return new DebugLogger(name, context as Object, (_, level) => level >= MinimumLogLevel);
#else
            return new DebugLogger(name, (_, level) => level >= MinimumLogLevel);
#endif
        }

        private static ILogger CreateForRuntime(string name, object context)
        {
#if UNITY_5_5_OR_NEWER
            return new DebugLogger(
                name,
                context as Object,
                (_, level) => (level >= MinimumLogLevel) && (level >= LogLevel.Information));
#else
            return new DebugLogger(
                name,
                (_, level) => (level >= MinimumLogLevel) && (level >= LogLevel.Information));
#endif
        }

        private static ILogger CreateForTesting(string name, object context)
        {
            return NullLogger.Instance;
        }

        #endregion

        #region Nested Types

        private class NullLogger : ILogger
        {
            private NullLogger()
            {
            }

            public static NullLogger Instance { get; } = new NullLogger();

            public IDisposable BeginScope<TState>(TState state)
            {
                return Disposable.Empty;
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return false;
            }

            public void Log<TState>(LogLevel logLevel,
                                    EventId eventId,
                                    TState state,
                                    Exception exception,
                                    Func<TState, Exception, string> formatter)
            {
            }
        }

        #endregion
    }
}