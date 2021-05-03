namespace Caliburn.Noesis
{
    #region Using Directives

    using System;
    using Object = UnityEngine.Object;

    #endregion

    /// <summary>
    ///     Helper class responsible for creating a logger for an instance.
    /// </summary>
    public static class LogManager
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the factory method for an <see cref="ILogger" />.
        /// </summary>
        public static Func<object, ILogger> GetLogger { get; set; } = context => new UnityConsoleLogger(context);

        #endregion

        #region Public Methods

        /// <summary>
        ///     Gets a <see cref="ILogger" /> implementation which does doing nothing.
        /// </summary>
        /// <remarks>Use this to override the <see cref="GetLogger" /> factory to deactivate logging.</remarks>
        /// <returns>A <see cref="ILogger" /> implementation which does nothing.</returns>
        public static ILogger GetNullLogger()
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

            /// <summary>
            ///     Static instance of this logger implementation.
            /// </summary>
            public static NullLogger Instance { get; } = new NullLogger();

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
            public void Assert(Func<bool> test, Object context, string message, params object[] args)
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
            public void Debug(Exception exception, Object context, string message, params object[] args)
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
            public void Error(Exception exception, Object context, string message, params object[] args)
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
            public void Fatal(Exception exception, Object context, string message, params object[] args)
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
            public void Info(Exception exception, Object context, string message, params object[] args)
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
            public void Trace(Exception exception, Object context, string message, params object[] args)
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
            public void Warn(Exception exception, Object context, string message, params object[] args)
            {
            }
        }

        #endregion
    }
}