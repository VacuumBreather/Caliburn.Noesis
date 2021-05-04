namespace Caliburn.Noesis
{
    #region Using Directives

    using System;
#if UNITY_5_5_OR_NEWER
    using Object = UnityEngine.Object;

#endif

    #endregion

    /// <summary>
    ///     A <see cref="ILogger" /> implementation which writes to
    ///     <see cref="System.Diagnostics.Debug" />.
    /// </summary>
    public class DebugConsoleLogger : ILogger
    {
        #region ILogger Implementation

        /// <inheritdoc />
        public void Assert(bool test, string message, params object[] args)
        {
            if (!test)
            {
                System.Diagnostics.Debug.WriteLine("[Assert] " + message, args);
            }
        }

        /// <inheritdoc />
        public void Assert(bool test, Object context, string message, params object[] args)
        {
            if (!test)
            {
                System.Diagnostics.Debug.WriteLine("[Assert] " + message, args);
            }
        }

        /// <inheritdoc />
        public void Assert(Func<bool> test, string message, params object[] args)
        {
            if (!test())
            {
                System.Diagnostics.Debug.WriteLine("[Assert] " + message, args);
            }
        }

        /// <inheritdoc />
        public void Assert(Func<bool> test, Object context, string message, params object[] args)
        {
            if (!test())
            {
                System.Diagnostics.Debug.WriteLine("[Assert] " + message, args);
            }
        }

        /// <inheritdoc />
        public void Debug(string message, params object[] args)
        {
            System.Diagnostics.Debug.WriteLine("[Debug] " + message, args);
        }

        /// <inheritdoc />
        public void Debug(Object context, string message, params object[] args)
        {
            System.Diagnostics.Debug.WriteLine("[Debug] " + message, args);
        }

        /// <inheritdoc />
        public void Debug(Exception exception, string message, params object[] args)
        {
            System.Diagnostics.Debug.WriteLine("[Debug] " + message, args);
        }

        /// <inheritdoc />
        public void Debug(Exception exception, Object context, string message, params object[] args)
        {
            System.Diagnostics.Debug.WriteLine("[Debug] " + message, args);
        }

        /// <inheritdoc />
        public void Error(string message, params object[] args)
        {
            System.Diagnostics.Debug.WriteLine("[Error] " + message, args);
        }

        /// <inheritdoc />
        public void Error(Object context, string message, params object[] args)
        {
            System.Diagnostics.Debug.WriteLine("[Error] " + message, args);
        }

        /// <inheritdoc />
        public void Error(Exception exception, string message, params object[] args)
        {
            System.Diagnostics.Debug.WriteLine("[Error] " + message, args);
        }

        /// <inheritdoc />
        public void Error(Exception exception, Object context, string message, params object[] args)
        {
            System.Diagnostics.Debug.WriteLine("[Error] " + message, args);
        }

        /// <inheritdoc />
        public void Fatal(string message, params object[] args)
        {
            System.Diagnostics.Debug.WriteLine("[Fatal] " + message, args);
        }

        /// <inheritdoc />
        public void Fatal(Object context, string message, params object[] args)
        {
            System.Diagnostics.Debug.WriteLine("[Fatal] " + message, args);
        }

        /// <inheritdoc />
        public void Fatal(Exception exception, string message, params object[] args)
        {
            System.Diagnostics.Debug.WriteLine("[Fatal] " + message, args);
        }

        /// <inheritdoc />
        public void Fatal(Exception exception, Object context, string message, params object[] args)
        {
            System.Diagnostics.Debug.WriteLine("[Fatal] " + message, args);
        }

        /// <inheritdoc />
        public void Info(string message, params object[] args)
        {
            System.Diagnostics.Debug.WriteLine("[Info] " + message, args);
        }

        /// <inheritdoc />
        public void Info(Object context, string message, params object[] args)
        {
            System.Diagnostics.Debug.WriteLine("[Info] " + message, args);
        }

        /// <inheritdoc />
        public void Info(Exception exception, string message, params object[] args)
        {
            System.Diagnostics.Debug.WriteLine("[Info] " + message, args);
        }

        /// <inheritdoc />
        public void Info(Exception exception, Object context, string message, params object[] args)
        {
            System.Diagnostics.Debug.WriteLine("[Info] " + message, args);
        }

        /// <inheritdoc />
        public void Trace(string message, params object[] args)
        {
            System.Diagnostics.Debug.WriteLine("[Trace] " + message, args);
        }

        /// <inheritdoc />
        public void Trace(Object context, string message, params object[] args)
        {
            System.Diagnostics.Debug.WriteLine("[Trace] " + message, args);
        }

        /// <inheritdoc />
        public void Trace(Exception exception, string message, params object[] args)
        {
            System.Diagnostics.Debug.WriteLine("[Trace] " + message, args);
        }

        /// <inheritdoc />
        public void Trace(Exception exception, Object context, string message, params object[] args)
        {
            System.Diagnostics.Debug.WriteLine("[Trace] " + message, args);
        }

        /// <inheritdoc />
        public void Warn(string message, params object[] args)
        {
            System.Diagnostics.Debug.WriteLine("[Warn] " + message, args);
        }

        /// <inheritdoc />
        public void Warn(Object context, string message, params object[] args)
        {
            System.Diagnostics.Debug.WriteLine("[Warn] " + message, args);
        }

        /// <inheritdoc />
        public void Warn(Exception exception, string message, params object[] args)
        {
            System.Diagnostics.Debug.WriteLine("[Warn] " + message, args);
        }

        /// <inheritdoc />
        public void Warn(Exception exception, Object context, string message, params object[] args)
        {
            System.Diagnostics.Debug.WriteLine("[Warn] " + message, args);
        }

        #endregion
    }
}