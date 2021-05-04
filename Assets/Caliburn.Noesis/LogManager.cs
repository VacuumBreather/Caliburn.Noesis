namespace Caliburn.Noesis
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
#if UNITY_5_5_OR_NEWER
    using UnityEngine;
    using Object = UnityEngine.Object;

#endif

    #endregion

    /// <summary>Responsible for creating various <see cref="ILogger" /> types.</summary>
    public static class LogManager
    {
        #region Constants and Fields

        private static readonly Dictionary<string, ILogger> StaticLoggers =
            new Dictionary<string, ILogger>();

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the function which creates a <see cref="ILogger" /> for the given context
        ///     intended for development in the editor.
        /// </summary>
        /// <returns>
        ///     A function which creates a <see cref="ILogger" /> for the given context intended for
        ///     development in the editor.
        /// </returns>
        public static Func<object, ILogger> CreateForEditor { get; set; } = context =>
#if UNITY_5_5_OR_NEWER
            new UnityConsoleLogger(context);
#else
            new DebugConsoleLogger(context);
#endif

        /// <summary>
        ///     Gets or sets the function which creates a <see cref="ILogger" /> for the given context
        ///     intended for runtime use.
        /// </summary>
        /// <returns>
        ///     A function which creates a <see cref="ILogger" /> for the given context intended for
        ///     runtime use.
        /// </returns>
        public static Func<object, ILogger> CreateForRuntime { get; set; } = context =>
#if UNITY_5_5_OR_NEWER
            new UnityConsoleLogger(context);
#else
            new DebugConsoleLogger(context);
#endif

        /// <summary>
        ///     Gets or sets the function which creates a <see cref="ILogger" /> for the given context
        ///     intended for testing only.
        /// </summary>
        /// <returns>
        ///     A function which creates a <see cref="ILogger" /> for the given context intended for
        ///     testing only.
        /// </returns>
        public static Func<object, ILogger> CreateForTesting { get; set; } =
            context => NullLogger.Instance;

        #endregion

        #region Public Methods

        /// <summary>Creates a <see cref="ILogger" /> for the specified context.</summary>
        /// <param name="context">
        ///     The context of the logger. This should be a game object or a type for non
        ///     unity classes.
        /// </param>
        /// <returns>The logger for the specified context.</returns>
        public static ILogger CreateLogger(object context)
        {
            if (!(context is Type type))
            {
                return CreateLoggerInternal(context);
            }

            if (StaticLoggers.TryGetValue(type.Name, out var staticLogger))
            {
                return staticLogger;
            }

            staticLogger = CreateLoggerInternal(type.Name);

            StaticLoggers[type.Name] = staticLogger;

            return staticLogger;
        }

        #endregion

        #region Private Methods

        private static ILogger CreateLoggerInternal(object context)
        {
            if (UnitTestDetector.IsInUnitTest)
            {
                return CreateForTesting?.Invoke(context) ?? NullLogger.Instance;
            }

#if UNITY_5_5_OR_NEWER
            return Application.isEditor
                       ? CreateForEditor?.Invoke(context) ?? NullLogger.Instance
                       : CreateForRuntime?.Invoke(context) ?? NullLogger.Instance;
#else
            return CreateForEditor?.Invoke(context) ?? NullLogger.Instance;
#endif
        }

        #endregion

        #region Nested Types

        private class NullLogger : ILogger
        {
            private NullLogger()
            {
            }

            /// <summary>Gets the static singleton instance of this logger.</summary>
            public static ILogger Instance { get; } = new NullLogger();

            /// <inheritdoc />
            public void Assert(bool test, string message, params object[] args)
            {
            }

            /// <inheritdoc />
            public void Assert(bool test, Object context, string message, params object[] args)
            {
            }

            /// <inheritdoc />
            public void Assert(Func<bool> test, string message, params object[] args)
            {
            }

            /// <inheritdoc />
            public void Assert(Func<bool> test,
                               Object context,
                               string message,
                               params object[] args)
            {
            }

            /// <inheritdoc />
            public void Debug(string message, params object[] args)
            {
            }

            /// <inheritdoc />
            public void Debug(Object context, string message, params object[] args)
            {
            }

            /// <inheritdoc />
            public void Debug(Exception exception, string message, params object[] args)
            {
            }

            /// <inheritdoc />
            public void Debug(Exception exception,
                              Object context,
                              string message,
                              params object[] args)
            {
            }

            /// <inheritdoc />
            public void Error(string message, params object[] args)
            {
            }

            /// <inheritdoc />
            public void Error(Object context, string message, params object[] args)
            {
            }

            /// <inheritdoc />
            public void Error(Exception exception, string message, params object[] args)
            {
            }

            /// <inheritdoc />
            public void Error(Exception exception,
                              Object context,
                              string message,
                              params object[] args)
            {
            }

            /// <inheritdoc />
            public void Fatal(string message, params object[] args)
            {
            }

            /// <inheritdoc />
            public void Fatal(Object context, string message, params object[] args)
            {
            }

            /// <inheritdoc />
            public void Fatal(Exception exception, string message, params object[] args)
            {
            }

            /// <inheritdoc />
            public void Fatal(Exception exception,
                              Object context,
                              string message,
                              params object[] args)
            {
            }

            /// <inheritdoc />
            public void Info(string message, params object[] args)
            {
            }

            /// <inheritdoc />
            public void Info(Object context, string message, params object[] args)
            {
            }

            /// <inheritdoc />
            public void Info(Exception exception, string message, params object[] args)
            {
            }

            /// <inheritdoc />
            public void Info(Exception exception,
                             Object context,
                             string message,
                             params object[] args)
            {
            }

            /// <inheritdoc />
            public void Trace(string message, params object[] args)
            {
            }

            /// <inheritdoc />
            public void Trace(Object context, string message, params object[] args)
            {
            }

            /// <inheritdoc />
            public void Trace(Exception exception, string message, params object[] args)
            {
            }

            /// <inheritdoc />
            public void Trace(Exception exception,
                              Object context,
                              string message,
                              params object[] args)
            {
            }

            /// <inheritdoc />
            public void Warn(string message, params object[] args)
            {
            }

            /// <inheritdoc />
            public void Warn(Object context, string message, params object[] args)
            {
            }

            /// <inheritdoc />
            public void Warn(Exception exception, string message, params object[] args)
            {
            }

            /// <inheritdoc />
            public void Warn(Exception exception,
                             Object context,
                             string message,
                             params object[] args)
            {
            }
        }

        #endregion
    }
}