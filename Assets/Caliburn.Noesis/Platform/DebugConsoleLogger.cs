#if !UNITY_5_5_OR_NEWER
namespace Caliburn.Noesis
{
    using System;

    /// <summary>
    ///     A <see cref="ILogger" /> implementation which writes to
    ///     <see cref="System.Diagnostics.Debug" />.
    /// </summary>
    public class DebugConsoleLogger : ILogger
    {
        #region Constants and Fields

        private readonly object ctx;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="DebugConsoleLogger" /> class.</summary>
        /// <param name="context">The context the logger should operate on.</param>
        public DebugConsoleLogger(object context)
        {
            this.ctx = context;
        }

        #endregion

        #region ILogger Implementation

        /// <inheritdoc />
        public void Assert(bool test, string message, params object[] args)
        {
            Assert(test, this.ctx, message, args);
        }

        /// <inheritdoc />
        public void Assert(bool test, object context, string message, params object[] args)
        {
            if (!test)
            {
                System.Diagnostics.Debug.WriteLine($"[Assert]{message}", args);
            }
        }

        /// <inheritdoc />
        public void Assert(Func<bool> test, string message, params object[] args)
        {
            Assert(test, this.ctx, message, args);
        }

        /// <inheritdoc />
        public void Assert(Func<bool> test, object context, string message, params object[] args)
        {
            if (!test())
            {
                System.Diagnostics.Debug.WriteLine($"[Assert]{message}", args);
            }
        }

        /// <inheritdoc />
        public void Debug(string message, params object[] args)
        {
            Debug(this.ctx, message, args);
        }

        /// <inheritdoc />
        public void Debug(object context, string message, params object[] args)
        {
            System.Diagnostics.Debug.WriteLine($"[Debug]{message}", args);
        }

        /// <inheritdoc />
        public void Debug(Exception exception, string message, params object[] args)
        {
            Debug(exception, this.ctx, message, args);
        }

        /// <inheritdoc />
        public void Debug(Exception exception, object context, string message, params object[] args)
        {
            System.Diagnostics.Debug.WriteLine($"{exception}");
        }

        /// <inheritdoc />
        public void Error(string message, params object[] args)
        {
            Error(this.ctx, message, args);
        }

        /// <inheritdoc />
        public void Error(object context, string message, params object[] args)
        {
            System.Diagnostics.Debug.WriteLine($"[Error]{message}", args);
        }

        /// <inheritdoc />
        public void Error(Exception exception, string message, params object[] args)
        {
            Error(exception, this.ctx, message, args);
        }

        /// <inheritdoc />
        public void Error(Exception exception, object context, string message, params object[] args)
        {
            System.Diagnostics.Debug.WriteLine($"{exception}");
        }

        /// <inheritdoc />
        public void Fatal(string message, params object[] args)
        {
            Fatal(this.ctx, message, args);
        }

        /// <inheritdoc />
        public void Fatal(object context, string message, params object[] args)
        {
            System.Diagnostics.Debug.WriteLine($"[Fatal]{message}", args);
        }

        /// <inheritdoc />
        public void Fatal(Exception exception, string message, params object[] args)
        {
            Fatal(exception, this.ctx, message, args);
        }

        /// <inheritdoc />
        public void Fatal(Exception exception, object context, string message, params object[] args)
        {
            System.Diagnostics.Debug.WriteLine($"{exception}");
        }

        /// <inheritdoc />
        public void Info(string message, params object[] args)
        {
            Info(this.ctx, message, args);
        }

        /// <inheritdoc />
        public void Info(object context, string message, params object[] args)
        {
            System.Diagnostics.Debug.WriteLine($"[Info]{message}", args);
        }

        /// <inheritdoc />
        public void Info(Exception exception, string message, params object[] args)
        {
            Info(exception, this.ctx, message, args);
        }

        /// <inheritdoc />
        public void Info(Exception exception, object context, string message, params object[] args)
        {
            System.Diagnostics.Debug.WriteLine($"{exception}");
        }

        /// <inheritdoc />
        public void Trace(string message, params object[] args)
        {
            Trace(this.ctx, message, args);
        }

        /// <inheritdoc />
        public void Trace(object context, string message, params object[] args)
        {
            System.Diagnostics.Debug.WriteLine($"[Trace]{message}", args);
        }

        /// <inheritdoc />
        public void Trace(Exception exception, string message, params object[] args)
        {
            Trace(exception, this.ctx, message, args);
        }

        /// <inheritdoc />
        public void Trace(Exception exception, object context, string message, params object[] args)
        {
            System.Diagnostics.Debug.WriteLine($"{exception}");
        }

        /// <inheritdoc />
        public void Warn(string message, params object[] args)
        {
            Warn(this.ctx, message, args);
        }

        /// <inheritdoc />
        public void Warn(object context, string message, params object[] args)
        {
            System.Diagnostics.Debug.WriteLine($"[Warn]{message}", args);
        }

        /// <inheritdoc />
        public void Warn(Exception exception, string message, params object[] args)
        {
            Warn(exception, this.ctx, message, args);
        }

        /// <inheritdoc />
        public void Warn(Exception exception, object context, string message, params object[] args)
        {
            System.Diagnostics.Debug.WriteLine($"{exception}");
        }

        #endregion
    }
}
#endif