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

    /// <summary>A logger writing to the standard debug output.</summary>
    [PublicAPI]
    public class DebugLogger : ILogger
    {
        #region Constants and Fields

        private readonly string categoryName;
        private readonly LogLevel minimumLogLevel;

#if UNITY_5_5_OR_NEWER
        private readonly Object context;
#endif

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="DebugLogger" /> class.</summary>
        /// <param name="categoryName">The category this logger is used by.</param>
        /// <param name="minimumLogLevel">The minimum <see cref="LogLevel" /> this logger will handle.</param>
        public DebugLogger(string categoryName, LogLevel minimumLogLevel = LogLevel.Information)
        {
            this.minimumLogLevel = minimumLogLevel;
            this.categoryName = string.IsNullOrEmpty(categoryName) ? nameof(DebugLogger) : categoryName;
        }

#if UNITY_5_5_OR_NEWER
        /// <summary>Initializes a new instance of the <see cref="DebugLogger" /> class.</summary>
        /// <param name="categoryName">The category this logger is used by.</param>
        /// <param name="context">The unity object that is the context of logged messages.</param>
        /// <param name="minimumLogLevel">The minimum <see cref="LogLevel" /> this logger will handle.</param>
        public DebugLogger(string categoryName, Object context, LogLevel minimumLogLevel = LogLevel.Information)
            : this(categoryName, minimumLogLevel)
        {
            this.context = context;
        }
#endif

        #endregion

        #region ILogger Implementation

        /// <summary>Scoped logging is not supported by this logger.</summary>
        IDisposable ILogger.BeginScope<TState>(TState state)
        {
            return Disposable.Empty;
        }

        /// <inheritdoc />
        public bool IsEnabled(LogLevel logLevel)
        {
#if UNITY_5_5_OR_NEWER
            return Application.isEditor &&
#else
                return System.Diagnostics.Debugger.IsAttached &&
#endif
                   (logLevel != LogLevel.None) &&
                   (logLevel >= this.minimumLogLevel);
        }

        /// <inheritdoc />
        public void Log<TState>(LogLevel logLevel,
                                EventId eventId,
                                TState state,
                                Exception exception,
                                Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var message = formatter(state, exception);

            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            message = $"[{this.categoryName}] [{logLevel}] {message}";

#if UNITY_5_5_OR_NEWER
            switch (logLevel)
            {
                case LogLevel.Trace:
                case LogLevel.Debug:
                case LogLevel.Information:
                    Debug.Log(message, this.context);

                    break;
                case LogLevel.Warning:
                    Debug.LogWarning(message, this.context);

                    break;
                case LogLevel.Error:
                    Debug.LogError(message, this.context);

                    break;
                case LogLevel.Critical:
                    Debug.LogError(message, this.context);

                    break;
            }
#else
                System.Diagnostics.Debug.WriteLine(message);
#endif
        }

        #endregion
    }
}