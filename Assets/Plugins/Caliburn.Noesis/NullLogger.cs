namespace Caliburn.Noesis
{
    #region Using Directives

    using System;
    using Object = UnityEngine.Object;

    #endregion

    /// <summary>
    ///     Implementation of <see cref="ILogger" /> which does nothing.
    /// </summary>
    /// <remarks>
    ///     Only meant for use in testing and as a fallback.
    /// </remarks>
    public class NullLogger : ILogger
    {
        #region Constructors and Destructors

        private NullLogger()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Static instance of this logger implementation.
        /// </summary>
        public static NullLogger Instance { get; } = new NullLogger();

        #endregion

        #region ILogger Implementation

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

        #endregion
    }
}