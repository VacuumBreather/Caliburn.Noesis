namespace Caliburn.Noesis.Transitions
{
    using System;
    using System.Windows;
    using System.Windows.Markup;
    using System.Windows.Media.Animation;

    /// <summary>Base class for transition effects.</summary>
    public abstract class TransitionEffectBase<TEffect> : MarkupExtension, ITransitionEffect
        where TEffect : ITransitionEffect, new()
    {
        #region ITransitionEffect Implementation

        /// <inheritdoc />
        public TimeSpan Delay { get; set; } = TimeSpan.Zero;

        /// <inheritdoc />
        public TimeSpan Duration { get; set; } = TimeSpan.FromMilliseconds(500);

        /// <inheritdoc />
        public IEasingFunction EasingFunction { get; set; } = new SineEase();

        /// <inheritdoc />
        public abstract Timeline Build<TSubject>(TSubject effectSubject)
            where TSubject : FrameworkElement, ITransitionEffectSubject;

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new TEffect();
        }

        #endregion
    }
}