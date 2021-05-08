namespace Caliburn.Noesis.Samples.ConductorOneActive
{
    using System;
    using System.Linq;
    using Extensions;
    using ViewModels;

#if UNITY_5_5_OR_NEWER
    using UnityEngine;

#endif

    /// <summary>Bootstrapper for the Conductor.OneActive sample.</summary>
#if UNITY_5_5_OR_NEWER
    [AddComponentMenu("Bootstrappers/Conductor.OneActive Bootstrapper")]
#endif
    public class ConductorOneActiveBootstrapper : BootstrapperBase<MainViewModel>
    {
        #region Protected Methods

        /// <inheritdoc />
        protected override MainViewModel GetMainContentViewModel()
        {
            var subScreens = GetConfiguration()
                             .AssemblySource.ViewModelTypes.Where(
                                 type => type.IsDerivedFromOrImplements(typeof(ISubScreen)))
                             .Select(Activator.CreateInstance)
                             .Cast<ISubScreen>();

            return new MainViewModel(subScreens);
        }

        #endregion
    }
}