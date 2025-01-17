using System;
#if UNITY_5_5_OR_NEWER
using UnityEngine;
#else
using System.Diagnostics;
#endif

namespace Caliburn.Noesis
{
    /// <summary>
    ///   A simple logger thats logs everything to the debugger.
    /// </summary>
    public class DebugLog : ILog
    {
        private readonly string typeName;

        /// <summary>
        /// Initializes a new instance of the <see cref="DebugLog"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public DebugLog(Type type)
        {
            typeName = type.FullName;
        }

        /// <summary>
        /// Logs the message as info.
        /// </summary>
        /// <param name="format">A formatted message.</param>
        /// <param name="args">Parameters to be injected into the formatted message.</param>
        public void Info(string format, params object[] args)
        {
#if UNITY_5_5_OR_NEWER
            Debug.Log($"[{typeName}] INFO: {string.Format(format, args)}");
#else
            Trace.WriteLine($"[{typeName}] INFO: {string.Format(format, args)}");
#endif
        }

        /// <summary>
        /// Logs the message as a warning.
        /// </summary>
        /// <param name="format">A formatted message.</param>
        /// <param name="args">Parameters to be injected into the formatted message.</param>
        public void Warn(string format, params object[] args)
        {
#if UNITY_5_5_OR_NEWER
            Debug.LogWarning($"[{typeName}] WARN: {string.Format(format, args)}");
#else
            Trace.WriteLine($"[{typeName}] WARN: {string.Format(format, args)}");
#endif
        }

        /// <summary>
        /// Logs the exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public void Error(Exception exception)
        {
#if UNITY_5_5_OR_NEWER
            Debug.LogError($"[{typeName}] ERROR: {exception}");
#else
            Trace.WriteLine($"[{typeName}] ERROR: {exception}" );
#endif
        }
    }
}
