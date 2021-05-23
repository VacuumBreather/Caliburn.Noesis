namespace Caliburn.Noesis.Transitions
{
    using System;
    using System.Windows;
    using System.Windows.Markup;
    using System.Windows.Media.Animation;

    /// <summary>Base class for transition effects.</summary>
    public abstract class TransitionBase : MarkupExtension, ITransition
    {
        #region ITransition Implementation

        /// <inheritdoc />
        public TimeSpan Delay { get; set; } = TimeSpan.Zero;

        /// <inheritdoc />
        public TimeSpan Duration { get; set; } = TimeSpan.FromMilliseconds(500);

        /// <inheritdoc />
        public IEasingFunction EasingFunction { get; set; } = new SineEase();

        /// <inheritdoc />
        public Point Origin { get; set; } = new Point(0.5, 0.5);

        /// <inheritdoc />
        public abstract Timeline Build<TSubject>(TSubject effectSubject)
            where TSubject : FrameworkElement, ITransitionSubject;

        /// <inheritdoc />
        public abstract void Cancel<TSubject>(TSubject effectSubject)
            where TSubject : FrameworkElement, ITransitionSubject;

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        #endregion
    }
}