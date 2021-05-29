namespace Caliburn.Noesis.Samples.HelloWorld
{
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
    }
}