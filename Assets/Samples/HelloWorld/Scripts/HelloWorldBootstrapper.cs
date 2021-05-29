namespace Caliburn.Noesis.Samples.HelloWorld
{
    using Microsoft.Extensions.Logging;
    using ViewModels;
#if UNITY_5_5_OR_NEWER
    using UnityEngine;

#endif

    /// <summary>The bootstrapper for the Hello World sample.</summary>
#if UNITY_5_5_OR_NEWER
    [AddComponentMenu("Bootstrappers/Hello World Bootstrapper")]
#endif
    public class HelloWorldBootstrapper : BootstrapperBase<MainViewModel>
    {
        #region Protected Methods

        /// <summary>Override this to add custom behavior on initialization.</summary>
        protected override void OnInitialize()
        {
#if UNITY_5_5_OR_NEWER
            LogManager.SetLoggerFactory(new DebugLoggerFactory(this, LogLevel.Debug));
#else
            LogManager.SetLoggerFactory(new DebugLoggerFactory(LogLevel.Trace));
#endif
        }

        #endregion
    }
}