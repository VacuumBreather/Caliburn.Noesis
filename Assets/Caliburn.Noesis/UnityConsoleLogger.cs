namespace Caliburn.Noesis
{
    #region Using Directives

    using System;
    using Object = UnityEngine.Object;

    #endregion

    /// <summary>A logger implementation which only logs to the unity console.</summary>
    public class UnityConsoleLogger : ILogger
    {
        #region Constants and Fields

        private readonly object ctx;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="UnityConsoleLogger" /> class.</summary>
        /// <param name="context">The context the logger should operate on.</param>
        public UnityConsoleLogger(object context)
        {
            this.ctx = GetFinalContext(context);
        }

        #endregion

        #region ILogger Implementation

        /// <inheritdoc />
        public void Assert(bool test, string message, params object[] args)
        {
            Assert(test, this.ctx as Object, message, args);
        }

        /// <inheritdoc />
        public void Assert(bool test, Object context, string message, params object[] args)
        {
            if (!test)
            {
                UnityEngine.Debug.LogAssertionFormat(context, $"[Assert]{message}", args);
            }
        }

        /// <inheritdoc />
        public void Assert(Func<bool> test, string message, params object[] args)
        {
            Assert(test, this.ctx as Object, message, args);
        }

        /// <inheritdoc />
        public void Assert(Func<bool> test, Object context, string message, params object[] args)
        {
            if (!test())
            {
                UnityEngine.Debug.LogAssertionFormat(context, $"[Assert]{message}", args);
            }
        }

        /// <inheritdoc />
        public void Debug(string message, params object[] args)
        {
            Debug(this.ctx as Object, message, args);
        }

        /// <inheritdoc />
        public void Debug(Object context, string message, params object[] args)
        {
            UnityEngine.Debug.LogFormat(context, $"[Debug]{message}", args);
        }

        /// <inheritdoc />
        public void Debug(Exception exception, string message, params object[] args)
        {
            Debug(exception, this.ctx as Object, message, args);
        }

        /// <inheritdoc />
        public void Debug(Exception exception, Object context, string message, params object[] args)
        {
            UnityEngine.Debug.LogException(exception, context);
        }

        /// <inheritdoc />
        public void Error(string message, params object[] args)
        {
            Error(this.ctx as Object, message, args);
        }

        /// <inheritdoc />
        public void Error(Object context, string message, params object[] args)
        {
            UnityEngine.Debug.LogErrorFormat(context, $"[Error]{message}", args);
        }

        /// <inheritdoc />
        public void Error(Exception exception, string message, params object[] args)
        {
            Error(exception, this.ctx as Object, message, args);
        }

        /// <inheritdoc />
        public void Error(Exception exception, Object context, string message, params object[] args)
        {
            UnityEngine.Debug.LogException(exception, context);
        }

        /// <inheritdoc />
        public void Fatal(string message, params object[] args)
        {
            Fatal(this.ctx as Object, message, args);
        }

        /// <inheritdoc />
        public void Fatal(Object context, string message, params object[] args)
        {
            UnityEngine.Debug.LogErrorFormat(context, $"[Fatal]{message}", args);
        }

        /// <inheritdoc />
        public void Fatal(Exception exception, string message, params object[] args)
        {
            Fatal(exception, this.ctx as Object, message, args);
        }

        /// <inheritdoc />
        public void Fatal(Exception exception, Object context, string message, params object[] args)
        {
            UnityEngine.Debug.LogException(exception, context);
        }

        /// <inheritdoc />
        public void Info(string message, params object[] args)
        {
            Info(this.ctx as Object, message, args);
        }

        /// <inheritdoc />
        public void Info(Object context, string message, params object[] args)
        {
            UnityEngine.Debug.LogFormat(context, $"[Info]{message}", args);
        }

        /// <inheritdoc />
        public void Info(Exception exception, string message, params object[] args)
        {
            Info(exception, this.ctx as Object, message, args);
        }

        /// <inheritdoc />
        public void Info(Exception exception, Object context, string message, params object[] args)
        {
            UnityEngine.Debug.LogException(exception, context);
        }

        /// <inheritdoc />
        public void Trace(string message, params object[] args)
        {
            Trace(this.ctx as Object, message, args);
        }

        /// <inheritdoc />
        public void Trace(Object context, string message, params object[] args)
        {
            UnityEngine.Debug.LogFormat(context, $"[Trace]{message}", args);
        }

        /// <inheritdoc />
        public void Trace(Exception exception, string message, params object[] args)
        {
            Trace(exception, this.ctx as Object, message, args);
        }

        /// <inheritdoc />
        public void Trace(Exception exception, Object context, string message, params object[] args)
        {
            UnityEngine.Debug.LogException(exception, context);
        }

        /// <inheritdoc />
        public void Warn(string message, params object[] args)
        {
            Warn(this.ctx as Object, message, args);
        }

        /// <inheritdoc />
        public void Warn(Object context, string message, params object[] args)
        {
            UnityEngine.Debug.LogWarningFormat(context, $"[Warn]{message}", args);
        }

        /// <inheritdoc />
        public void Warn(Exception exception, string message, params object[] args)
        {
            Warn(exception, this.ctx as Object, message, args);
        }

        /// <inheritdoc />
        public void Warn(Exception exception, Object context, string message, params object[] args)
        {
            UnityEngine.Debug.LogException(exception, context);
        }

        #endregion

        #region Private Methods

        private static object GetFinalContext(object context)
        {
            return context is Type type ? type.Name : context;
        }

        #endregion
    }
}