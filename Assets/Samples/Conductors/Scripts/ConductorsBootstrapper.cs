namespace Caliburn.Noesis.Samples.Conductors
{
    using System;
    using System.Linq;
    using Extensions;
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