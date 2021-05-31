namespace Caliburn.Noesis
{
    using System;
    using System.Collections.Generic;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Logging;
    using ILogger = Microsoft.Extensions.Logging.ILogger;
#if UNITY_5_5_OR_NEWER
    using UnityEngine;
    using Object = UnityEngine.Object;

#endif

    /// <summary>A simple <see cref="ILoggerFactory" /> for logging to the debug output.</summary>
    [PublicAPI]
    public class DebugLoggerFactory : ILoggerFactory
    {
        #region Constants and Fields

        private readonly List<WeakReference<DebugLogger>> loggers =
            new List<WeakReference<DebugLogger>>();

#if UNITY_5_5_OR_NEWER
        private readonly Object context;
#endif

        private LogLevel logLevel;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="DebugLoggerFactory" /> class.</summary>
        /// <param name="logLevel">
        ///     The minimal <see cref="LogLevel" /> the loggers created by this factory will
        ///     handle.
        /// </param>
        public DebugLoggerFactory(LogLevel logLevel)
        {
            this.logLevel = logLevel;
        }

#if UNITY_5_5_OR_NEWER
        /// <summary>Initializes a new instance of the <see cref="DebugLoggerFactory" /> class.</summary>
        /// <param name="context">
        ///     The <see cref="UnityEngine" />.<see cref="UnityEngine.Object" /> which
        ///     defines the context of all loggers created by this factory.
        /// </param>
        /// <param name="logLevel">
        ///     The minimal <see cref="LogLevel" /> the loggers created by this factory will
        ///     handle.
        /// </param>
        public DebugLoggerFactory(Object context, LogLevel logLevel)
            : this(logLevel)
        {
            this.context = context;
        }
#endif

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the minimal <see cref="LogLevel" /> the loggers created by this factory will
        ///     handle.
        /// </summary>
        public LogLevel LogLevel
        {
            get => this.logLevel;
            set
            {
                if (this.logLevel == value)
                {
                    return;
                }

                this.logLevel = value;

                foreach (var reference in this.loggers)
                {
                    if (reference.TryGetTarget(out var logger))
                    {
                        logger.MinimalLogLevel = this.logLevel;
                    }
                }
            }
        }

        #endregion

        #region IDisposable Implementation

        /// <inheritdoc />
        void IDisposable.Dispose()
        {
        }

        #endregion

        #region ILoggerFactory Implementation

        /// <inheritdoc />
        void ILoggerFactory.AddProvider(ILoggerProvider provider)
        {
        }

        /// <inheritdoc />
        public ILogger CreateLogger(string categoryName)
        {
#if UNITY_5_5_OR_NEWER
            var logger = new DebugLogger(categoryName, this.context, LogLevel);
#else
            var logger = new DebugLogger(categoryName, LogLevel);
#endif
            this.loggers.Add(new WeakReference<DebugLogger>(logger));

            return logger;
        }

        #endregion

        #region Nested Types

        private class DebugLogger : ILogger
        {
            private readonly string categoryName;

#if UNITY_5_5_OR_NEWER
            private readonly Object context;
#endif
            private readonly Func<string, LogLevel, bool> filter;

            public DebugLogger(string categoryName, LogLevel minimumLogLevel = LogLevel.Information)
            {
                this.categoryName = string.IsNullOrEmpty(categoryName)
                                        ? nameof(DebugLogger)
                                        : categoryName;
                MinimalLogLevel = minimumLogLevel;
                this.filter = Filter;
            }

#if UNITY_5_5_OR_NEWER
            public DebugLogger(string categoryName,
                               Object context,
                               LogLevel minimumLogLevel = LogLevel.Information)
                : this(categoryName, minimumLogLevel)
            {
                this.context = context;
            }
#endif

            public LogLevel MinimalLogLevel { get; set; }

            public IDisposable BeginScope<TState>(TState state)
            {
                return Disposable.Empty;
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                // If the filter is null, everything is enabled
#if UNITY_5_5_OR_NEWER

                // unless we are running in the editor.
                return Application.isEditor &&
#else
                // unless the debugger is not attached.
                return System.Diagnostics.Debugger.IsAttached &&
#endif
                       (logLevel != LogLevel.None) &&
                       (this.filter?.Invoke(this.categoryName, logLevel) ?? true);
            }

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

#if UNITY_5_5_OR_NEWER
                var color = GetLogColor<TState>(logLevel);

                message =
                    $"<color={color}><b>[{this.categoryName}] [{logLevel}]</b> {message}</color>";

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

                if (exception != null)
                {
                    Debug.LogException(exception, this.context);
                }
#else
                message = $"[{logLevel}] {message}";

                if (exception != null)
                {
                    message += Environment.NewLine + Environment.NewLine + exception;
                }

                System.Diagnostics.Debug.WriteLine(message, this.categoryName);
#endif
            }

#if UNITY_5_5_OR_NEWER
            private static string GetLogColor<TState>(LogLevel logLevel)
            {
                var color = "#FFFFFFFF";

                switch (logLevel)
                {
                    case LogLevel.Trace:
                        color = "#87CEFA";

                        break;
                    case LogLevel.Debug:
                        color = "#FF69B4";

                        break;
                    case LogLevel.Information:
                        color = "#1E90FF";

                        break;
                    case LogLevel.Warning:
                        color = "#FFD700";

                        break;
                    case LogLevel.Error:
                        color = "#FF4500";

                        break;
                    case LogLevel.Critical:
                        color = "#DC143C";

                        break;
                }

                return color;
            }
#endif

            private bool Filter(string _, LogLevel level)
            {
                return level >= MinimalLogLevel;
            }
        }

        #endregion
    }
}