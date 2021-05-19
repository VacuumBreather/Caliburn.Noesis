namespace Caliburn.Noesis.Transitions
{
    using System.Windows;
    using System.Windows.Media.Animation;

    /// <summary>Base class for transition effects.</summary>
    public abstract class TransitionEffectBase : FrameworkElement, ITransitionEffect
    {
        #region ITransitionEffect Implementation

        /// <inheritdoc />
        public abstract Timeline Build<TSubject>(TSubject effectSubject)
            where TSubject : FrameworkElement, ITransitionEffectSubject;

        #endregion
    }
}