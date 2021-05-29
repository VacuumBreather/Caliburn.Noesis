namespace Caliburn.Noesis.Samples
{
    using JetBrains.Annotations;
    using Microsoft.Extensions.Logging;
#if UNITY_5_5_OR_NEWER
    using UnityEngine;

#endif

    /// <summary>Provides a hook for minimum log level configuration.</summary>
    [PublicAPI]
    public static class LogConfigurator
    {
        #region Constants and Fields

        private static readonly LogLevel MinimumLogLevel = LogLevel.Debug;

        #endregion

        #region Public Methods

        /// <summary>Initializes the <see cref="LogConfigurator" /> class.</summary>
#if UNITY_5_5_OR_NEWER
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
#endif
        public static void SetMinimumLogLevel()
        {
            SetMinimumLogLevel(MinimumLogLevel);
        }

        #endregion

        #region Private Methods

        private static void SetMinimumLogLevel(LogLevel logLevel)
        {
            LogManager.MinimumLogLevel = logLevel;
        }

        #endregion
    }
}