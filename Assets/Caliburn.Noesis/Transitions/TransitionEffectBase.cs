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
        public abstract Timeline Build<TSubject>(TSubject effectSubject)
            where TSubject : FrameworkElement, ITransitionEffectSubject;

        #endregion

        #region Public Properties

        public TimeSpan Duration { get; set; } = TimeSpan.FromMilliseconds(400);

        public TimeSpan OffsetTime { get; set; } = TimeSpan.Zero;

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