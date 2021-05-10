namespace Caliburn.Noesis.Samples.Converters
{
    using ViewModels;
#if UNITY_5_5_OR_NEWER
    using UnityEngine;
#endif

    /// <summary>The bootstrapper for the Converters sample.</summary>
#if UNITY_5_5_OR_NEWER
    [AddComponentMenu("Bootstrappers/Converters Bootstrapper")]
#endif
    public class ConvertersBootstrapper : BootstrapperBase<MainViewModel>
    {
    }
}