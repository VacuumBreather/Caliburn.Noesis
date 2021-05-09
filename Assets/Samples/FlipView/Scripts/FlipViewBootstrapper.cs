namespace Caliburn.Noesis.Samples.FlipView
{
    using ViewModels;
#if UNITY_5_5_OR_NEWER
    using UnityEngine;
#endif

    /// <summary>The bootstrapper for the FlipView sample.</summary>
#if UNITY_5_5_OR_NEWER
    [AddComponentMenu("Bootstrappers/FlipView Bootstrapper")]
#endif
    public class FlipViewBootstrapper : BootstrapperBase<MainViewModel>
    {
    }
}