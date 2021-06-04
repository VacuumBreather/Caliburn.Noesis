namespace Caliburn.Noesis.Samples.Conductors
{
    using ViewModels;
#if UNITY_5_5_OR_NEWER
    using UnityEngine;

#endif

    /// <summary>Bootstrapper for the Conductors sample.</summary>
#if UNITY_5_5_OR_NEWER
    [AddComponentMenu("Bootstrappers/Conductors Bootstrapper")]
#endif
    public class ConductorsBootstrapper : BootstrapperBase<MainViewModel>
    {
    }
}