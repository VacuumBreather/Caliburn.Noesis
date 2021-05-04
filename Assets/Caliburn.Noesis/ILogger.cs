namespace Caliburn.Noesis
{
    #region Using Directives

    using System;
    using Object = UnityEngine.Object;

    #endregion

    /// <summary>Interface for a logger.</summary>
    public interface ILogger
    {
        /// <summary>Logs an assertion message if the assertion fails.</summary>
        /// <param name="test">The assertion.</param>
        /// <param name="message">The assertion message.</param>
        /// <param name="args">The arguments of the message.</param>
        void Assert(bool test, string message, params object[] args);

        /// <summary>Logs an assertion message if the assertion fails.</summary>
        /// <param name="test">The assertion.</param>
        /// <param name="context">The context of the logged assertion message.</param>
        /// <param name="message">The assertion message.</param>
        /// <param name="args">The arguments of the message.</param>
        void Assert(bool test, Object context, string message, params object[] args);

        /// <summary>Logs an assertion message if the assertion fails.</summary>
        /// <param name="test">A function representing the assertion.</param>
        /// <param name="message">The assertion message.</param>
        /// <param name="args">The arguments of the message.</param>
        void Assert(Func<bool> test, string message, params object[] args);

        /// <summary>Logs an assertion message if the assertion fails.</summary>
        /// <param name="test">A function representing the assertion.</param>
        /// <param name="context">The context of the logged assertion message.</param>
        /// <param name="message">The assertion message.</param>
        /// <param name="args">The arguments of the message.</param>
        void Assert(Func<bool> test, Object context, string message, params object[] args);

        /// <summary>Logs a message at debug level.</summary>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments of the message.</param>
        void Debug(string message, params object[] args);

        /// <summary>Logs a message at debug level.</summary>
        /// <param name="context">The context of the logged message.</param>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments of the message.</param>
        void Debug(Object context, string message, params object[] args);

        /// <summary>Logs an exception at debug level.</summary>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments of the message.</param>
        void Debug(Exception exception, string message, params object[] args);

        /// <summary>Logs an exception at debug level.</summary>
        /// <param name="context">The context of the logged exception.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments of the message.</param>
        void Debug(Exception exception, Object context, string message, params object[] args);

        /// <summary>Logs a message at error level.</summary>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments of the message.</param>
        void Error(string message, params object[] args);

        /// <summary>Logs a message at error level.</summary>
        /// <param name="context">The context of the logged message.</param>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments of the message.</param>
        void Error(Object context, string message, params object[] args);

        /// <summary>Logs an exception at error level.</summary>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments of the message.</param>
        void Error(Exception exception, string message, params object[] args);

        /// <summary>Logs an exception at error level.</summary>
        /// <param name="context">The context of the logged exception.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments of the message.</param>
        void Error(Exception exception, Object context, string message, params object[] args);

        /// <summary>Logs a message at fatal level.</summary>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments of the message.</param>
        void Fatal(string message, params object[] args);

        /// <summary>Logs a message at fatal level.</summary>
        /// <param name="context">The context of the logged message.</param>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments of the message.</param>
        void Fatal(Object context, string message, params object[] args);

        /// <summary>Logs an exception at fatal level.</summary>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments of the message.</param>
        void Fatal(Exception exception, string message, params object[] args);

        /// <summary>Logs an exception at fatal level.</summary>
        /// <param name="context">The context of the logged exception.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments of the message.</param>
        void Fatal(Exception exception, Object context, string message, params object[] args);

        /// <summary>Logs a message at information level.</summary>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments of the message.</param>
        void Info(string message, params object[] args);

        /// <summary>Logs a message at information level.</summary>
        /// <param name="context">The context of the logged message.</param>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments of the message.</param>
        void Info(Object context, string message, params object[] args);

        /// <summary>Logs an exception at information level.</summary>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments of the message.</param>
        void Info(Exception exception, string message, params object[] args);

        /// <summary>Logs an exception at information level.</summary>
        /// <param name="context">The context of the logged exception.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments of the message.</param>
        void Info(Exception exception, Object context, string message, params object[] args);

        /// <summary>Logs a message at trace level.</summary>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments of the message.</param>
        void Trace(string message, params object[] args);

        /// <summary>Logs a message at trace level.</summary>
        /// <param name="context">The context of the logged message.</param>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments of the message.</param>
        void Trace(Object context, string message, params object[] args);

        /// <summary>Logs an exception at trace level.</summary>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments of the message.</param>
        void Trace(Exception exception, string message, params object[] args);

        /// <summary>Logs an exception at trace level.</summary>
        /// <param name="context">The context of the logged exception.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments of the message.</param>
        void Trace(Exception exception, Object context, string message, params object[] args);

        /// <summary>Logs a message at warning level.</summary>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments of the message.</param>
        void Warn(string message, params object[] args);

        /// <summary>Logs a message at warning level.</summary>
        /// <param name="context">The context of the logged message.</param>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments of the message.</param>
        void Warn(Object context, string message, params object[] args);

        /// <summary>Logs an exception at warning level.</summary>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments of the message.</param>
        void Warn(Exception exception, string message, params object[] args);

        /// <summary>Logs an exception at warning level.</summary>
        /// <param name="context">The context of the logged exception.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments of the message.</param>
        void Warn(Exception exception, Object context, string message, params object[] args);
    }
}