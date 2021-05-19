namespace Caliburn.Noesis.Samples.Transitions
{
    using ViewModels;
#if UNITY_5_5_OR_NEWER
    using UnityEngine;
#endif

    /// <summary>The bootstrapper for the Transitions sample.</summary>
#if UNITY_5_5_OR_NEWER
    [AddComponentMenu("Bootstrappers/Transitions Bootstrapper")]
#endif
    public class TransitionsBootstrapper : BootstrapperBase<MainViewModel>
    {
    }
}