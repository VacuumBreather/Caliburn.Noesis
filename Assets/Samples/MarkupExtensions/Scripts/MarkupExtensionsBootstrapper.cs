namespace Caliburn.Noesis.Samples.MarkupExtensions
{
    using ViewModels;
#if UNITY_5_5_OR_NEWER
    using UnityEngine;

#endif

    /// <summary>The bootstrapper for the MarkupExtensions sample.</summary>
#if UNITY_5_5_OR_NEWER
    [AddComponentMenu("Bootstrappers/MarkupExtensions Bootstrapper")]
#endif
    public class MarkupExtensionsBootstrapper : AutofacBootstrapperBase<MainViewModel>
    {
    }
}