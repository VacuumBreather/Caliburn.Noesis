namespace Caliburn.Noesis
{
    using System;
    using Microsoft.Extensions.Logging;
    using ILogger = Microsoft.Extensions.Logging.ILogger;
#if UNITY_5_5_OR_NEWER
    using UnityEngine;
    using Object = UnityEngine.Object;

#endif

    /// <summary>A logger that writes messages in the debug output window only when a debugger is attached.</summary>
    public class DebugLogger : ILogger
    {
        #region Constants and Fields

        private readonly Func<string, LogLevel, bool> filter;
        private readonly string name;

#if UNITY_5_5_OR_NEWER
        private readonly Object context;
#endif

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="DebugLogger" /> class.</summary>
        /// <param name="name">The name of the logger.</param>
        /// <param name="filter">(Optional) The function used to filter events based on the log level.</param>
        public DebugLogger(string name, Func<string, LogLevel, bool> filter = null)
        {
            this.name = string.IsNullOrEmpty(name) ? nameof(DebugLogger) : name;
            this.filter = filter;
        }

#if UNITY_5_5_OR_NEWER
        /// <summary>Initializes a new instance of the <see cref="DebugLogger" /> class.</summary>
        /// <param name="name">The name of the logger.</param>
        /// <param name="context">The object context of this logger.</param>
        /// <param name="filter">(Optional) The function used to filter events based on the log level.</param>
        public DebugLogger(string name, Object context, Func<string, LogLevel, bool> filter = null)
            : this(name, filter)
        {
            this.context = context;
        }
#endif

        #endregion

        #region ILogger Implementation

        /// <inheritdoc />
        public IDisposable BeginScope<TState>(TState state)
        {
            return Disposable.Empty;
        }

        /// <inheritdoc />
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
                   (this.filter?.Invoke(this.name, logLevel) ?? true);
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

            message = $"[{logLevel}] {message}";

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

            if (exception != null)
            {
                Debug.LogException(exception, this.context);
            }
#else
            if (exception != null)
            {
                message += Environment.NewLine + Environment.NewLine + exception;
            }

            System.Diagnostics.Debug.WriteLine(message, this.name);
#endif
        }

        #endregion
    }
}